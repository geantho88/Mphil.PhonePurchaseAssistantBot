using HtmlAgilityPack;
using Mphil.PhonePurchaseAssistantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Crawlers
{
    public class eshopgrCrawler
    {
        private readonly string _url;
        public eshopgrCrawler(string url)
        {
            _url = url;
        }

        public string FindProduct(UserProfile userProfile)
        {
            var url = "http://signup.wordpress.com/signup/";

            var htmlWeb = new HtmlWeb();
            var doc = htmlWeb.Load(_url);

            var value = doc.DocumentNode.SelectSingleNode("//form[@id='setupform']");
            Console.WriteLine(value.OuterHtml);
        }
    }
}
