namespace Models.Common.Policy.Calculation
{
    public class ElectricMotorcycleDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumForFirstThreshold { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicPremiumForAboveThreshold { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicAsPerKW { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel EcoFriendlyDiscount { get; set; } = new CalculationSubDetailAmountModel();
        public decimal KW { get; set; }
        public CalculationSubDetailAmountModel TPLAsPerKW { get; set; } = new CalculationSubDetailAmountModel();
    }
}
