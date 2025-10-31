namespace Models.Common.Policy.Calculation
{
    public class PrivateVehicleDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumForFirstThreshold { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicPremiumForAboveThreshold { get; set; }= new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicAsPerCC { get; set; }= new CalculationSubDetailAmountModel();
        public decimal CC { get; set; }
        public int NumberOfSeats { get; set; }
    }
}
