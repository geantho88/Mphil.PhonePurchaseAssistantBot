using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mphil.PhonePurchaseAssistantBot.Crawlers;
using Mphil.PhonePurchaseAssistantBot.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Mphil.PhonePurchaseAssistantBot.Tests
{
    [TestClass]
    public class CrawlerTests
    {
        [TestMethod]
        public void PublicCrawler_Can_Get_Smartphone_Results()
        {
            var userProfile = new UserProfile()
            {
                Age = 45,
                BudgetString = "250",
                Interests = "Running, Photography, Walking",
                Name = "John",
                Occupation = "Driver",
                PhoneTypeString = "Smartphone"
            };

            var eshopgrCrawler = new publicCrawler();
            var result = eshopgrCrawler.FindProducts(userProfile);

            Assert.IsTrue(result != null && result.Any());
        }

        [TestMethod]
        public void PublicCrawler_Can_Get_FeaturePhone_Results()
        {
            var userProfile = new UserProfile()
            {
                Age = 45,
                BudgetString = "250",
                Interests = "Running, Photography, Walking",
                Name = "John",
                Occupation = "Driver",
                PhoneTypeString = "Feature phone"
            };

            var eshopgrCrawler = new publicCrawler();
            var result = eshopgrCrawler.FindProducts(userProfile);

            Assert.IsTrue(result != null && result.Any());
        }
    }
}
