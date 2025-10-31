namespace Models.Common.Policy.Calculation.Miscellaneous.TR
{
    public class UnNamedIndividualTRCalculationModel : CommonIndividualTRCalculationModel
    {
        public int NumberofPerson { get; set; }
        public decimal Rate { get; set; }
        public string PrimaryId { get; set; }
        public decimal MedicalSumInsuredLimitPI { get; set; }
        public decimal SumInsuredPI { get; set; }
        public decimal AdditionalMedicalAmountPI { get; set; }
    }
}
