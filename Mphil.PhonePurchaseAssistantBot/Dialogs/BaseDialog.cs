using Microsoft.Bot.Builder.Dialogs;
using Mphil.PhonePurchaseAssistantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class BaseDialog : ComponentDialog
    {
        protected static UserProfile userProfile;
        protected const string userInfo = "value-userInfo";

        public BaseDialog(string v)
        {
            V = v;
        }

        public string V { get; }
    }
}
