using HtmlAgilityPack;
using Mphil.PhonePurchaseAssistantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Crawlers
{
    public class publicCrawler
    {
        public publicCrawler()
        {
        }

        public List<MobilePhone> FindProducts(UserProfile userProfile)
        {
            string url = "";

            if (userProfile.PhoneType == PhoneType.FeaturePhone)
            {
                if (userProfile.Budget <= 60)
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-aplis-hrisis/?N=864600736+1195434581+1476864300+4095975735&gclid=Cj0KCQiA6t6ABhDMARIsAONIYyyr4TeKimdF2k7hxJ3PF5LXDBJL6c_cLE8NUxerULfbAROZee0nzY8aAp2cEALw_wcB&origUrl=/cat/tilefonia/kinita-aplis-hrisis/";
                }
                else
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-aplis-hrisis/?N=864600736+1923357053+2595721522+4249361850+361940987&gclid=Cj0KCQiA6t6ABhDMARIsAONIYyyr4TeKimdF2k7hxJ3PF5LXDBJL6c_cLE8NUxerULfbAROZee0nzY8aAp2cEALw_wcB&origUrl=/cat/tilefonia/kinita-aplis-hrisis/";
                }
            }
            else
            {
                if (userProfile.Budget > 0 && userProfile.Budget <= 100)
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-smartphones/?N=2331662508+3082828756+2592577317+436022263&origUrl=/cat/tilefonia/kinita-smartphones/";
                }
                else if (userProfile.Budget > 100 && userProfile.Budget <= 200)
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-smartphones/?N=2331662508+3082828756+1623803296+3991193754&origUrl=/cat/tilefonia/kinita-smartphones/";
                }
                else if (userProfile.Budget > 200 && userProfile.Budget <= 400)
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-smartphones/?N=2331662508+3082828756+818010178+413744908+4015222960&origUrl=/cat/tilefonia/kinita-smartphones/";
                }
                else
                {
                    url = "https://www.public.gr/cat/tilefonia/kinita-smartphones/?N=2331662508+3082828756+1332046605+2248783647+1013394927+2184801566&origUrl=/cat/tilefonia/kinita-smartphones/";
                }
            }


            HtmlWeb htmlWeb = new HtmlWeb();

            var doc = htmlWeb.Load(url); 

            var items = doc.DocumentNode.SelectNodes("//div[@class='product-list']//div[@class='col-sm-6 col-lg-4']");

            List<MobilePhone> phones = new List<MobilePhone>();

            foreach (HtmlNode item in items)
            {
                try
                {
                    var brand = item.SelectSingleNode(".//a[@class='teaser--product-title product-page-link istile']")?.InnerText?.Trim();
                    var manufacturer = item.SelectSingleNode(".//div[@class='teaser--product-description']")?.InnerText?.Trim();
                    var producturl = $"https://www.public.gr{item.SelectSingleNode(".//div[@class='teaser--product-details']//a")?.Attributes["href"]?.Value}";
                    var price = item.SelectSingleNode(".//div[@class='teaser--product-final-price large']")?.Attributes["data-price"]?.Value;
                    if(string.IsNullOrEmpty(price))
                    {
                        price = item.SelectSingleNode(".//div[@class='teaser--product-final-price seller-price large']")?.Attributes["data-price"]?.Value;
                    }
                    var stock = item.SelectSingleNode(".//div[@class='product-delivery']").InnerText?.Trim();
                    var rating = item.SelectSingleNode(".//div[@class='reevooBadge']")?.InnerText?.Trim();
                    var imageurl = $"https://www.public.gr{item.SelectSingleNode(".//div[@class='teaser-image']//a")?.Attributes["href"]?.Value}";

                    var phone = new MobilePhone
                    {
                        Brand = brand,
                        Manufacturer = manufacturer,
                        Price = price,
                        Rating = rating ?? "5",
                        Stock = stock,
                        ProductUrl = producturl,
                        ImageUrl = imageurl
                    };

                    phones.Add(phone);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return phones;
        }
    }
}
