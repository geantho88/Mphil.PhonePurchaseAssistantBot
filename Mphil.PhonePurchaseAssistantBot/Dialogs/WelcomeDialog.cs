using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Mphil.PhonePurchaseAssistantBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class WelcomeDialog : BaseDialog
    {
        // Define value names for values tracked inside the dialogs.

        private readonly List<BotData> botData;
        private readonly Random rnd;

        public WelcomeDialog()
            : base(nameof(WelcomeDialog))
        {
            AddDialog(new TextPrompt("IntroAsync"));
            AddDialog(new TextPrompt("NameStepAsync"));
            AddDialog(new TextPrompt("AgeStepAsync"));
            AddDialog(new TextPrompt("OccupationStepAsync"));
            AddDialog(new TextPrompt("InterestsStepAsync"));
            AddDialog(new TextPrompt("SummarizeStepAsync"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroAsync,
                NameStepAsync,
                AgeStepAsync,
                OccupationStepAsync,
                InterestsStepAsync,
                SummarizeStepAsync,
            }));

            AddDialog(new UserRequirementsDialog());

            InitialDialogId = nameof(WaterfallDialog);

            string path = System.IO.Directory.GetCurrentDirectory();
            var botDataJson = System.IO.File.ReadAllText(path + "/Bots/BotData.json");
            botData = JsonConvert.DeserializeObject<List<BotData>>(botDataJson);

            rnd = new Random();
        }

        private async Task<DialogTurnResult> IntroAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var welcomeMessage = botData.SingleOrDefault(a => a.key == "Welcome Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            var responsetext = stepContext.Context.Activity.Text.ToLowerInvariant();

            if (DialogChecker.MessageContainsBadWords(responsetext))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var result = await LanguageDetectionMessageAsync(responsetext);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            if (responsetext.Contains("hello") || responsetext.Contains("hi"))
            {
                await stepContext.Context.SendActivityAsync($"{responsetext}! {welcomeMessage}", cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"you said: {responsetext}! That's Ok! " + welcomeMessage, cancellationToken: cancellationToken);
            }

            // Ask the user to enter their name.
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var askNameMessage = botData.SingleOrDefault(a => a.key == "Ask Name Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[userInfo] = new UserProfile();

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(askNameMessage) };

            // Ask the user to enter their name.
            return await stepContext.PromptAsync("AgeStepAsync", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (DialogChecker.MessageContainsBadWords((string)stepContext.Result))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var result = await LanguageDetectionMessageAsync((string)stepContext.Result);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var askAgeMessage = botData.SingleOrDefault(a => a.key == "Ask Age Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            // Set the user's name to what they entered in response to the name prompt.
            userProfile = (UserProfile)stepContext.Values[userInfo];

            userProfile.Name = (string)stepContext.Result;
            userProfile.Name = userProfile.Name.Replace("my name is", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                // Ask the user to enter their name.
                await stepContext.RepromptDialogAsync(cancellationToken);
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"{askAgeMessage} {userProfile.Name}?"),
            };

            // Ask the user to enter their age.
            return await stepContext.PromptAsync("OccupationStepAsync", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> OccupationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (DialogChecker.MessageContainsBadWords((string)stepContext.Result))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var result = await LanguageDetectionMessageAsync((string)stepContext.Result);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var ageNotValidMessage = botData.SingleOrDefault(a => a.key == "Age Not Valid Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            var askOccupationMessage = botData.SingleOrDefault(a => a.key == "Ask Occupation").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            var age = Regex.Match(stepContext.Result.ToString(), @"\d+").Value;

            if (string.IsNullOrEmpty(age))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(ageNotValidMessage), cancellationToken);
            }

            else
            {
                userProfile.Age = int.Parse(age);
                if (userProfile.Age >= 0 && userProfile.Age < 12)
                {
                    // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("It seems you are too young to review for a phone... Are your parents aware of that?"), cancellationToken);
                }
                else if (userProfile.Age <= 0)
                {
                    // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("This is not a valid age. Please enter your age"), cancellationToken);
                }
                else if (userProfile.Age >= 99)
                {
                    // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("I think you are kidding me. Definetely you look younger :-) ...."), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"So you are {userProfile.Age} And still look awesome!"), cancellationToken);
                }
            }

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(askOccupationMessage) };
            return await stepContext.PromptAsync("InterestsStepAsync", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> InterestsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (DialogChecker.MessageContainsBadWords((string)stepContext.Result))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var result = await LanguageDetectionMessageAsync((string)stepContext.Result);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var professionsMatched = await DetectKeyPhrases((string)stepContext.Result);

            var askInterestsMessage = botData.SingleOrDefault(a => a.key == "Ask Interests Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            // Set the user's age to what they entered in response to the age prompt.
            userProfile.Occupation = professionsMatched;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks for your reply"), cancellationToken);

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(askInterestsMessage) };
            return await stepContext.PromptAsync("SummarizeStepAsync", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> SummarizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (DialogChecker.MessageContainsBadWords((string)stepContext.Result))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            var hobbiesMatched = await DetectKeyPhrases((string)stepContext.Result);

            // Set the user's age to what they entered in response to the age prompt.
            userProfile.Interests = hobbiesMatched;

            StringBuilder s = new StringBuilder();
            s.Append($"To sum up, your name is {userProfile.Name} ");

            if (userProfile.Age > 0)
            {
                s.Append($"you are {userProfile.Age} years old, ");
            }
            else
            {
                s.Append("You do not want to share your age with me, ");
            }

            s.Append($"you are working as {userProfile.Occupation} ");
            s.Append($"and your hobbies are {userProfile.Interests}");

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(s.ToString()), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Let's find what i can do for you in purchasing an awesome phone which suits 100% your needs"), cancellationToken);

            return await stepContext.BeginDialogAsync(nameof(UserRequirementsDialog), null, cancellationToken);
        }
    }
}
