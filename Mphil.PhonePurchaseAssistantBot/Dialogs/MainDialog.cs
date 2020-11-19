using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly UserState _userState;

        public MainDialog(UserState userState)
            : base(nameof(MainDialog))
        {
            _userState = userState;

            AddDialog(new WelcomeDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(WelcomeDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var userInfo = (UserProfile)stepContext.Result;

            //string status = "You are signed up to review "
            //    + (userInfo.CompaniesToReview.Count is 0 ? "no companies" : string.Join(" and ", userInfo.CompaniesToReview))
            //    + ".";

            //await stepContext.Context.SendActivityAsync(status);

            //var accessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            //await accessor.SetAsync(stepContext.Context, userInfo, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
