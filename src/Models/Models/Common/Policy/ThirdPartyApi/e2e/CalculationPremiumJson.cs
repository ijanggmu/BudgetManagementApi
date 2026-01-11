using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public interface ICalculationPremiumJson
    {
        string PortfolioAlias { get; set; }
        decimal SumInsured { get; set; }
        decimal BasicPremium { get; set; }
        decimal ThirdPartyPremium { get; set; }
        decimal RSMDTPremium { get; set; }
        decimal PersonalAccidentPremium { get; set; }
        decimal GrossPremium { get; set; }
        decimal TotalPremium { get; set; }
        decimal StampDuty { get; set; }
        decimal VatAmount { get; set; }
        decimal NetPremium { get; set; }
        decimal GovernmentSubsidyAmount { get; set; }
        decimal PayableAmount { get; set; }
        
    }
}
