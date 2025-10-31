namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class IndividualCriticalIllnessCalculationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal EmployeePremium { get; set; }
        public FamilyInfo Spouse { get; set; } = new FamilyInfo();
        public FamilyInfo Parent1 { get; set; } = new FamilyInfo();
        public FamilyInfo Parent2 { get; set; } = new FamilyInfo();
        public FamilyInfo Child1 { get; set; } = new FamilyInfo();
        public FamilyInfo Child2 { get; set; } = new FamilyInfo();
        public FamilyInfo Child3 { get; set; } = new FamilyInfo();
    }
    public class FamilyInfo
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Premium { get; set; }
    }
}