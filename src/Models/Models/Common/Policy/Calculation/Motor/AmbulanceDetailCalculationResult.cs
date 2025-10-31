namespace Models.Common.Policy.Calculation
{
    public class AmbulanceDetailCalculationResult : CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel TPLAsPerSeatCapacity { get; set; } = new CalculationSubDetailAmountModel();
        public int NumberOfSeats { get; set; }


    }
}
