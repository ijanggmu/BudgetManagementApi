using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public class CalculationPremium:ICalculationPremiumJson
    {
        public decimal SumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal ThirdPartyPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal PersonalAccidentPremium { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal StampDuty { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NetPremium { get; set; }
        public decimal GovernmentSubsidyAmount { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
