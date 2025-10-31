using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class FidelityGuaranteeDetailCalculationResult : CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel Burglary { get; set; }
        public CalculationSubDetailAmountModel FidelityGuarantee { get; set; }

    }
}
