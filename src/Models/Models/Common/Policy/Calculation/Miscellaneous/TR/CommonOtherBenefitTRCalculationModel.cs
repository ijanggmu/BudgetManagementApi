using System;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.Calculation.Miscellaneous.TR
{
    public class CommonOtherBenefitTRCalculationModel : OtherBenefitCommonProperties
    {
        public decimal OtherPremiumAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }

}