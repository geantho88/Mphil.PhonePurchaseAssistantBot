using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Mphil.PhonePurchaseAssistantBot.Crawlers;
using Mphil.PhonePurchaseAssistantBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class UserRequirementsDialog : BaseDialog
    {
        private readonly List<BotData> botData;
        private readonly List<string> badWords;
        private readonly Random rnd;
        // Define value names for values tracked inside the dialogs.
        private const string userRequirements = "value-userRequirements";

        public UserRequirementsDialog()
            : base(nameof(UserRequirementsDialog))
        {
            AddDialog(new TextPrompt("SelectTypeOfPhoneAsync"));
            AddDialog(new TextPrompt("SelectBudgetAsync"));
            AddDialog(new TextPrompt("StartSelectionStepAsync"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SelectTypeOfPhoneAsync,
                SelectBudgetAsync,
                StartSelectionStepAsync,
                //AcknowledgementStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);

            string path = System.IO.Directory.GetCurrentDirectory();
            var botDataJson = System.IO.File.ReadAllText(path + "/Bots/BotData.json");
            var badWorkdsJson = System.IO.File.ReadAllText(path + "/Dictionaries/badwords.json");

            botData = JsonConvert.DeserializeObject<List<BotData>>(botDataJson);
        }

        private async Task<DialogTurnResult> SelectTypeOfPhoneAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (DialogChecker.MessageContainsBadWords((string)stepContext.Result))
            {
                var reply = botData.SingleOrDefault(a => a.key == "Bad Words Reply Generic Messages").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                await stepContext.Context.SendActivityAsync(reply, cancellationToken: cancellationToken);
                await stepContext.EndDialogAsync();
            }

            var result = await LanguageDetectionMessageAsync((string)stepContext.Result);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            await stepContext.Context.SendActivityAsync($"Do you prefer Smartphones or feature phones? Let me explain before you decide...", cancellationToken: cancellationToken);

            await stepContext.Context.SendActivityAsync($"Smartphones are distinguished from feature phones by their stronger hardware capabilities and extensive mobile operating systems, which facilitate wider software, internet", cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.ContentUrl("https://www.e-shop.gr/images/TEL/BIG/TEL.092396.jpg", MediaTypeNames.Image.Jpeg, "Smartphone", "This is how a smartphone looks like"));

            await stepContext.Context.SendActivityAsync($"Feature phone that retains the form factor of earlier-generation phones, with button-based input and a small display. Feature phones are sometimes called dumbphones in contrast with touch-input smartphones.", cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.ContentUrl("https://www.e-shop.gr/images/TEL/BIG/TEL.051078.jpg", MediaTypeNames.Image.Jpeg, "Feature phone", "This is how a feature phone looks like"));

            await stepContext.Context.SendActivityAsync($"Note that smartphones are usually more expensive compared to feature phones", cancellationToken: cancellationToken);

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "What is your decision?",
                    SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                        new CardAction() { Title = "Smartphone", Type = ActionTypes.ImBack, Value = "I want a smartphone" },
                        new CardAction() { Title = "Feature phone", Type = ActionTypes.ImBack, Value = "I want a feature phone" },
                         },
                    },
                }
            };

            // Ask the user to enter their name.
            return await stepContext.PromptAsync("SelectBudgetAsync", opts, cancellationToken);
        }

        private async Task<DialogTurnResult> SelectBudgetAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var smartphoneTypeSelected = (string)stepContext.Result;

            var phoneType = smartphoneTypeSelected == "I want a smartphone" ? "Smartphone" : "Feature Phone";
            userProfile.PhoneTypeString = phoneType;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You prefer {phoneType}!, that's a good choice"), cancellationToken);

            var askBudgetMessage = botData.SingleOrDefault(a => a.key == "Ask Budget Message").values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(askBudgetMessage) };
            return await stepContext.PromptAsync("StartSelectionStepAsync", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> StartSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var budget = (string)stepContext.Result.ToString();

            var result = await LanguageDetectionMessageAsync(budget);
            if (!string.IsNullOrEmpty(result))
            {
                await stepContext.Context.SendActivityAsync(result, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

            budget = Regex.Match(budget, @"\d+").Value;

            double budgVael = 0;
            double.TryParse(budget, out budgVael);

            userProfile.BudgetString = budget;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks for your valuable information. I am looking for the best choice!!"), cancellationToken);

            var eshopgrCrawler = new publicCrawler();
            var results = eshopgrCrawler.FindProducts(userProfile);

            results = results.OrderByDescending(a => a.Rating).ThenBy(x => x.Price).ToList();

            var resultPhone = results.FirstOrDefault(a => a.Stock == "διαθέσιμο" && double.Parse(a.Price.Replace(",",".")) <= budgVael);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Dear {userProfile.Name}, I suggest you to buy the '{resultPhone.Brand}' {userProfile.PhoneTypeString} as it is currently available for purchase (in stock), it has a very high rating from users {resultPhone.Rating} out of 10 and it is within your budget range at {resultPhone.Price} € only! Check and buy now from this link your new Phone {resultPhone.ProductUrl}"), cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
