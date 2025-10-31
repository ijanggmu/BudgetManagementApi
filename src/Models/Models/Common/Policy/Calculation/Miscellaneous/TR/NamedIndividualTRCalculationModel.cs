namespace Models.Common.Policy.Calculation.Miscellaneous.TR
{
    public class NamedIndividualTRCalculationModel : CommonIndividualTRCalculationModel
    {
        public string Name { get; set; }
        public string PrimaryId { get; set; }
        public decimal MedicalBenefitPremiumForAdministrativeStaff{ get; set; }  

    }
}
