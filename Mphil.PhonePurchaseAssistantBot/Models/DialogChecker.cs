using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Models
{
    public class DialogChecker
    {
        public static bool MessageContainsBadWords(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string path = System.IO.Directory.GetCurrentDirectory();
                string badWorkdsJson = System.IO.File.ReadAllText(path + "/Dictionaries/badwords.json");

                List<string> badWords = JsonConvert.DeserializeObject<List<string>>(badWorkdsJson);
                ;
                foreach (var badWord in badWords)
                {
                    var splittedWords = badWord.Split(' ');

                    foreach (var splittedWord in splittedWords)
                    {
                        var splittedMessageWords = message.Split(' ');
                        foreach (var splittedMessageWord in splittedMessageWords)
                        {
                            if (splittedMessageWord.Length >= 4 && splittedMessageWord.Equals(splittedWord))
                            {
                                var wordFound = splittedWord;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
