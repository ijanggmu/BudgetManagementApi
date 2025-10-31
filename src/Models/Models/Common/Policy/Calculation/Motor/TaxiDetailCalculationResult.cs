namespace Models.Common.Policy.Calculation
{
    public class TaxiDetailCalculationResult : CommonDetailsProperty
    {
        public int NumberOfSeats { get; set; }
        public decimal CC { get; set; }
        public CalculationSubDetailAmountModel BasicAsPerCC { get; set; } = new CalculationSubDetailAmountModel();
    }
}
