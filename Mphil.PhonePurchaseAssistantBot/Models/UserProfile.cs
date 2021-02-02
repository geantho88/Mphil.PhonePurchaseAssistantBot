using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mphil.PhonePurchaseAssistantBot.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public string Interests { get; set; }
        public string PhoneTypeString { get; set; }
        public string BudgetString { get; set; }
        public double Budget => !string.IsNullOrEmpty(BudgetString)? double.Parse(BudgetString) : 0;
        public PhoneType PhoneType => PhoneTypeString == "Smartphone"? PhoneType.Smartphone : PhoneType.FeaturePhone;
    }
}
