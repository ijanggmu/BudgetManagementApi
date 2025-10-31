using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class ClassDetailRequest
    {
        public DateTime RegistrationDate { get; set; }
        public decimal CurrentMarketPrice { get; set; }
        public string PortfolioAlias { get; set; }
    }

    public class CalculateExcess : CalculateAgeAndDepreciation
    {
        public decimal? VoluntaryExcess { get; set; }
    }
    public class CalculateCurrentMarketPrice : CalculateAgeAndDepreciation
    {
        public decimal SumInsured { get; set; }
    }
    public class CalculateAgeAndDepreciation
    {
        public string PortfolioAlias { get; set; }
        public bool NewOrOldVehicle { get; set; }
        public string DateOfPurchase { get; set; }
        public string RegistrationDate { get; set; }
    }
}
