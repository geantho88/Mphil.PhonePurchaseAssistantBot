using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Bots
{
    public class WelcomeBot<T> : DialogBot<T> where T : Dialog
    {

        // Messages sent to the user.
        private const string WelcomeMessage = "This is a simple Welcome Bot sample. This bot will introduce you " +
                                                "to welcoming and greeting users. You can say 'intro' to see the " +
                                                "introduction card. If you are running this bot in the Bot Framework " +
                                                "Emulator, press the 'Start Over' button to simulate user joining " +
                                                "a bot or a channel";


        public WelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var introMessage1 = MessageFactory.Text($"Welcome to Phone Purchase Assistant Bot. ");
                    var introMessage2 = MessageFactory.Text("I am here to assist you in providing information which will help you to decide the purchase of your new mobile phone.");
                    var introMessage3 = MessageFactory.Text("Type anything to get started.");

                    await turnContext.SendActivityAsync(introMessage1, cancellationToken);
                    await turnContext.SendActivityAsync(introMessage2, cancellationToken);
                    await turnContext.SendActivityAsync(introMessage3, cancellationToken);

                }
            }
        }
    }
}
