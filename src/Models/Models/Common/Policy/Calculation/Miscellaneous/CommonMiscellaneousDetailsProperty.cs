using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel CalculatedOwnDamagePremium { get; set; }
        public CalculationSubDetailAmountModel BasicPremium { get; set; }
        public CalculationSubDetailAmountModel RSMD { get; set; }
        public CalculationSubDetailAmountModel Terrorism { get; set; }
        public CalculationSubDetailAmountModel DirectDiscount { get; set; }
        public CalculationSubDetailAmountModel Stamp { get; set; }
        public CalculationSubDetailAmountModel AdditionalRiskPremium { get; set; }


        //remove below properties
        //public CalculationSubDetailAmountModel BasicProRataOrShortScale { get; set; }

        public decimal SumInsured { get; set; }
        public int Days { get; set; }
        public decimal ShortScaleRate { get; set; }
    }
}
