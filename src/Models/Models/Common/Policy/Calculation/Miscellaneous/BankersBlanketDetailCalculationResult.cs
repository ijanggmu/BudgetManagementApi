using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class BankersBlanketDetailCalculationResult : CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel InitialRSMDT { get; set; }
        public CalculationSubDetailAmountModel InitialBasicPremium { get; set; }
        public CalculationSubDetailAmountModel BasicPremiumAdditionalClauseA { get; set; }
        public CalculationSubDetailAmountModel RSMDTPremiumAdditionalClauseA { get; set; }
        public CalculationSubDetailAmountModel RSMDTPremiumAdditionalClauseB { get; set; }
        public CalculationSubDetailAmountModel BasicPremiumAdditionalClauseB { get; set; }
        public CalculationSubDetailAmountModel LoadingForDishonestyOfEmployees { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
    }
}
