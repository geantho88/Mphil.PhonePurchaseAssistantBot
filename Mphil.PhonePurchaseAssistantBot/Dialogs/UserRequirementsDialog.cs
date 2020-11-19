using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class UserRequirementsDialog : ComponentDialog
    {

        // Define value names for values tracked inside the dialogs.
        private const string userRequirements = "value-userRequirements";

        public UserRequirementsDialog()
            : base(nameof(UserRequirementsDialog))
        {
            AddDialog(new TextPrompt("SelectTypeOfPhoneAsync"));
            AddDialog(new TextPrompt("SelectBudgetAsync"));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SelectTypeOfPhoneAsync,
                SelectBudgetAsync,
                //StartSelectionStepAsync,
                //AcknowledgementStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> SelectTypeOfPhoneAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Do you prefer Smartphones or feature phones? Let me explain before you decide...", cancellationToken: cancellationToken);
            
            await stepContext.Context.SendActivityAsync($"Smartphones are distinguished from feature phones by their stronger hardware capabilities and extensive mobile operating systems, which facilitate wider software, internet", cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.ContentUrl("https://www.e-shop.gr/images/TEL/BIG/TEL.092396.jpg", MediaTypeNames.Image.Jpeg, "Smartphone", "This is how a smartphone looks like"));

            await stepContext.Context.SendActivityAsync($"Feature phone that retains the form factor of earlier-generation phones, with button-based input and a small display. Feature phones are sometimes called dumbphones in contrast with touch-input smartphones.", cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.ContentUrl("https://www.e-shop.gr/images/TEL/BIG/TEL.051078.jpg", MediaTypeNames.Image.Jpeg, "Feature phone", "This is how a feature phone looks like"));

            await stepContext.Context.SendActivityAsync($"Note that smartphones are usually more expensive compared to feature phones", cancellationToken: cancellationToken);


            await stepContext.Context.SendActivityAsync(MessageFactory.SuggestedActions(new List<string> { "Smartphone", "Feature phone" }, "What is your decision?"));

            // Ask the user to enter their name.
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> SelectBudgetAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync();
        }

        //private async Task<DialogTurnResult> StartSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    // Set the user's age to what they entered in response to the age prompt.
        //    var userProfile = (UserProfile)stepContext.Values[UserInfo];
        //    userProfile.Age = (int)stepContext.Result;

        //    if (userProfile.Age < 25)
        //    {
        //        // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
        //        await stepContext.Context.SendActivityAsync(
        //            MessageFactory.Text("You must be 25 or older to participate."),
        //            cancellationToken);
        //        return await stepContext.NextAsync(new List<string>(), cancellationToken);
        //    }
        //    else
        //    {
        //        // Otherwise, start the review selection dialog.
        //        return await stepContext.BeginDialogAsync(nameof(ReviewSelectionDialog), null, cancellationToken);
        //    }
        //}

        //private async Task<DialogTurnResult> AcknowledgementStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    // Set the user's company selection to what they entered in the review-selection dialog.
        //    var userProfile = (UserProfile)stepContext.Values[UserInfo];
        //    userProfile.CompaniesToReview = stepContext.Result as List<string> ?? new List<string>();

        //    // Thank them for participating.
        //    await stepContext.Context.SendActivityAsync(
        //        MessageFactory.Text($"Thanks for participating, {((UserProfile)stepContext.Values[UserInfo]).Name}."),
        //        cancellationToken);

        //    // Exit the dialog, returning the collected user information.
        //    return await stepContext.EndDialogAsync(stepContext.Values[UserInfo], cancellationToken);
        //}
    }
}
