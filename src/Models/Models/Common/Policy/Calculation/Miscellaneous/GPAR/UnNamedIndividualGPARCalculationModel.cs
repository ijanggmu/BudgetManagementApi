namespace Models.Common.Policy.Calculation.Miscellaneous.GPAR
{
    public class UnNamedIndividualGPARCalculationModel : CommonIndividualGPARCalculationModel
    {
        public int NumberofPerson { get; set; }
        public decimal Rate { get; set; }
        public string PrimaryId { get; set; }
    }
}
