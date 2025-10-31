using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Agriculture
{
    public class CommonAgricultureDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel CalculatedOwnDamagePremium { get; set; }
        public CalculationSubDetailAmountModel GovernmentSubsidy { get; set; }
        public CalculationSubDetailAmountModel BalancedAmount { get; set; }
        public CalculationSubDetailAmountModel PAOfInsured { get; set; }
        public CalculationSubDetailAmountModel NCD { get; set; }
        public CalculationSubDetailAmountModel CorporateDiscount { get; set; }
        public CalculationSubDetailAmountModel BasicPremium { get; set; }
        public CalculationSubDetailAmountModel RSMD { get; set; }
        public CalculationSubDetailAmountModel Terrorism { get; set; }
        public CalculationSubDetailAmountModel DirectDiscount { get; set; }
        public CalculationSubDetailAmountModel Stamp { get; set; }
        public CalculationSubDetailAmountModel Balance { get; set; }
        public CalculationSubDetailAmountModel PaOfInusurer { get; set; }


        //remove below properties
        //public CalculationSubDetailAmountModel BasicProRataOrShortScale { get; set; }

        public decimal SumInsured { get; set; }
        public int Days { get; set; }
        public decimal ShortScaleRate { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
    }
}
