using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Models
{
    public partial class TranslationObject
    {
        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }

    public partial class Translation
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}
