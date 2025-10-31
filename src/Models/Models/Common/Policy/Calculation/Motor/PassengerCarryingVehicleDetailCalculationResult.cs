namespace Models.Common.Policy.Calculation
{
    public class PassengerCarryingVehicleDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel PaidToHelper { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PaidHelperRSMDT { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LessAsPerSeatsCapacity { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicAsPerSeatCapacity { get; set; } = new CalculationSubDetailAmountModel();
        public int NumberOfSeats { get; set; }
        public decimal RSMDTSumInsuredForHelper { get; set; }
    }
}
