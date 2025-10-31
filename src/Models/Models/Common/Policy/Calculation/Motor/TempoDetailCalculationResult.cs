using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Motor
{
    public class TempoDetailCalculationResult : CommonDetailsProperty
    {
        public int NumberOfSeats { get; set; }
        public decimal CC { get; set; }
        public CalculationSubDetailAmountModel BasicAsPerCC { get; set; } = new CalculationSubDetailAmountModel();
    }
}
