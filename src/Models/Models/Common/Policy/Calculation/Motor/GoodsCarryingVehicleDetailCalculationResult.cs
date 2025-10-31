namespace Models.Common.Policy.Calculation
{
    public class GoodsCarryingVehicleDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel ExcessMoreThan { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicAsPerCC { get; set; }= new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel CC { get; set; }= new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel TrailorAdditionDeduction { get; set; }= new CalculationSubDetailAmountModel();
        public int NumberOfSeats { get; set; }
        public decimal TailorValue { get; set; }
        public decimal VoluntaryExcessValue { get; set; }
        public decimal RSMDTSumInsuredAmountForDriver { get; set; }
        public decimal RSMDTSumInsuredAmountForHelper { get; set; }
        public decimal RSMDTSumInsuredAmountForPassengers { get; set; }

    }
}
