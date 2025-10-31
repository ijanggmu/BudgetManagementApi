using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Motor
{
    public class TractorDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicAsPerCC { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel CC { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel TrailorAdditionDeduction { get; set; } = new CalculationSubDetailAmountModel();
        public decimal TailorValue { get; set; }
        public decimal VoluntaryExcessValue { get; set; }
        public decimal RSMDTSumInsuredAmountForDriver { get; set; }
        public decimal RSMDTSumInsuredAmountForHelper { get; set; }
        public decimal RSMDTSumInsuredAmountForPassengers { get; set; }
    }
}
