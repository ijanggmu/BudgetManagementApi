namespace Models.Common.Policy.Calculation.Marine
{
    public class MarineIndividualCalculation
    {
        public string PrimaryId { get; set; }
        //Information of material
        public string ProductDescription { get; set; }
        // For Sum Insured Calculation
        public decimal InvoiceAmount { get; set; }
        public decimal ToleranceAmount { get; set; }
        public decimal AmountWithTolerance { get; set; }
        public decimal IncrementalCostAmount { get; set; }
        public decimal AmountWithIncrementalCost { get; set; }
        public decimal DutyAmount { get; set; }
        public decimal SumInsuredInCurrencyOfValue { get; set; }
        public decimal SumInsuredInNRS { get; set; }
        public decimal FullSumInsuredInNRS { get; set; }

        // For Premium Calculation
        public decimal TariffSchedulePremiumRate { get; set; }
        public decimal TariffSchedulePremiumAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
        public bool IsAdded { get; set; }
    }
}
