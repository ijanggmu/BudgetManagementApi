using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Engineering
{
    public class CommonEngineeringDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel RSMDTSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel TPLSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel CalculatedOwnDamagePremium { get; set; }
        public CalculationSubDetailAmountModel BasicPremium { get; set; }
        public CalculationSubDetailAmountModel DirectDiscount { get; set; }
        public CalculationSubDetailAmountModel RiotAndStrike { get; set; }
        public CalculationSubDetailAmountModel MD { get; set; }
        public CalculationSubDetailAmountModel Terrorism { get; set; }
        public CalculationSubDetailAmountModel RSMDT { get; set; }
        public CalculationSubDetailAmountModel AggregatedRSMDTAmount { get; set; }
        public CalculationSubDetailAmountModel InitialTPL { get; set; }
        public CalculationSubDetailAmountModel TPL { get; set; }

        public CalculationSubDetailAmountModel TPLDirectDiscount { get; set; }
        //remove below properties
        public CalculationSubDetailAmountModel BasicProRataOrShortScale { get; set; }
        public CalculationSubDetailAmountModel ThirdPartyProRataOrShortScale { get; set; }
        public CalculationSubDetailAmountModel RSMDTProRataOrShortScale { get; set; }

        public bool IsBasicPremiumSelected { get; set; }
        public bool IsTPLSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public decimal SumInsured { get; set; }
        public int Days { get; set; }
        public decimal ShortScaleRate { get; set; }
    }
}
