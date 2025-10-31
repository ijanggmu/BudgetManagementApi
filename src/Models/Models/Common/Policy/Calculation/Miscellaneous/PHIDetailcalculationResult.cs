using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class PHIDetailcalculationResult : CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel MedicalBenefit { get; set; }
        public bool IsDBasicSelected { get; set; }
        public bool IsEBasicSelected { get; set; }
        public bool IsRSMDSelected { get; set; }
    }
}
