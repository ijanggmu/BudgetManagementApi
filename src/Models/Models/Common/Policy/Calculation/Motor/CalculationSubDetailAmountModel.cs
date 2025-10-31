namespace Models.Common.Policy.Calculation
{
    public class CalculationSubDetailAmountModel
    {
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal ProRataOrShortScaleAmount { get; set; }
        public decimal Total { get; set; }
        //endorsement
        public decimal PreviousPaidAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public decimal PayableAmount { get; set; }

    }
}
