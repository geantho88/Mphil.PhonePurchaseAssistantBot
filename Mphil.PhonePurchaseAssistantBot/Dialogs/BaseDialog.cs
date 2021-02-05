using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Bot.Builder.Dialogs;
using Mphil.PhonePurchaseAssistantBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Dialogs
{
    public class BaseDialog : ComponentDialog
    {
        protected private readonly TextAnalyticsClient textAnalyticsClient;
        protected static UserProfile userProfile;
        protected const string userInfo = "value-userInfo";

        public BaseDialog(string v)
        {
            V = v;
            textAnalyticsClient = new TextAnalyticsClient(new Uri("https://mphiltextanalytics.cognitiveservices.azure.com/"), new AzureKeyCredential("d4425021a6974ea7ae9a9a6ea900b48d"));
        }

        public string V { get; }

        public async Task<string> DetectKeyPhrases(string text)
        {
            var response = await textAnalyticsClient.ExtractKeyPhrasesAsync(text);
            var items = response.Value?.ToList();
            string joined = string.Join(",", items);
            return joined;
        }

        public async Task<string> LanguageDetectionMessageAsync(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > 3)
            {
                DetectedLanguage detectedLanguage = textAnalyticsClient.DetectLanguage(text);

                if (detectedLanguage.Iso6391Name != "en")
                {
                    return await TranslateTextResponseAsync(detectedLanguage.Iso6391Name, "I am sorry I can't understand your language. I speak only English for the moment. Please try to communicate with me in English....exiting the application");
                }

                return null;
            }

            return null;
        }

        private async Task<string> TranslateTextResponseAsync(string tolanguage, string text)
        {
            // Input and output languages are defined as parameters.
            string route = $"/translate?api-version=3.0&from=en&to={tolanguage}";
            string textToTranslate = text;
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri("https://api.cognitive.microsofttranslator.com" + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", "92af997b4895422fbd7b3546ce5b88c5");
                request.Headers.Add("Ocp-Apim-Subscription-Region", "francecentral");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result =  await response.Content.ReadAsStringAsync();
                var translationsDetected = JsonConvert.DeserializeObject<List<TranslationObject>>(result);
                return translationsDetected?.FirstOrDefault()?.Translations?.FirstOrDefault().Text;
            }
        }
    }
}
