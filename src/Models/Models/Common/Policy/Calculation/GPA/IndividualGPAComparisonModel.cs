namespace Models.Common.Policy.Calculation.GPA
{
    public class IndividualGPAComparisonModel
    {
        public IndividualGPACalculationModel PreviousCalculation { get; set; }
        public IndividualGPACalculationModel CurrentCalculation { get; set; }
        public decimal PremiumDifference { get; set; }
        public decimal ProRataPremium { get; set; }
        public bool IsSumInsuredUpdated { get; set; }
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
    }
}
