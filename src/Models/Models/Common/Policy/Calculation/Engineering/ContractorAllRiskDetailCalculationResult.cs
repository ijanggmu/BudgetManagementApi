using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Engineering
{
    public class ContractorAllRiskDetailCalculationResult : CommonEngineeringDetailsProperty
    {
        public RSMDTProperties MaxRSMDT { get; set; }
        public RSMDTProperties SecondRSMDT { get; set; }
        public RSMDTProperties MinRSMDT { get; set; }
        public bool IsPremiumManual { get; set; }
    }
    public class RSMDTProperties
    {
        public CalculationSubDetailAmountModel AggregatedRSMDTAmount { get; set; }
        public CalculationSubDetailAmountModel RiotAndStrike { get; set; }
        public CalculationSubDetailAmountModel MD { get; set; }
        public CalculationSubDetailAmountModel Terrorism { get; set; }
        public CalculationSubDetailAmountModel RSMDTSpecialDiscount { get; set; }
        public CalculationSubDetailAmountModel RSMDT { get; set; }
    }
}
