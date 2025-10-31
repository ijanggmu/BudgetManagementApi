using Models.Common.Policy.Calculation.Miscellaneous;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class OldCreatePolicyViewModel
    {
        public OldHIPModel HospitalIndemnityPlanPartial { get; set; }
    }
    public class OldHIPModel
    {
        public List<InsuredPerson> InsuredPerson { get; set; }
        public decimal MaximumPerDay { get; set; }
        public decimal MaximumDays { get; set; }
        public decimal MaximumPerSpecifiedDays { get; set; }
        public decimal Minor { get; set; }
        public decimal Intermediate { get; set; }
        public decimal Major { get; set; }
        public decimal DoctorFee { get; set; }
        public decimal DoctorFeeToVisitPatientAtHome { get; set; }
        public decimal DoctorFeeLimit { get; set; }
        public decimal LimitOnCostOfMedicineMedicalAppliancesDressing { get; set; }
        public decimal LimitOnCostOfTypeOfCheckup { get; set; }
        public decimal FirstFourFeeLimit { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public decimal PremiumPerPerson { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal? ParentExtraCharge { get; set; }
        public bool IsHIPSelected { get; set; }
        public HIPPlans Employee { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndSpouse { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndFamily { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndParents { get; set; } = new HIPPlans();
        public HIPPlans EmployeeFamilyAndParents { get; set; } = new HIPPlans();
        public string[] Endorsements { get; set; }
        public string[] SpecialExclusions { get; set; }
        public string[] SpecificExtensions { get; set; }

    }
    public class InsuredPerson
    {
        public bool IsIncludedForMedical { get; set; }
        public string PersonalInformation { get; set; }
        public string Name { get; set; }
        public string Plan { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal Premium { get; set; }
        public string NatureOfOccupation { get; set; }
        public int Coverage { get; set; }
        public decimal SumInsured { get; set; }
        public string OfficialInformation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public InsuredPersonFamilyInfo Spouse { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child1 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child2 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child3 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child4 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child5 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child6 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child7 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child8 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child9 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Child10 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Parent1 { get; set; } = new InsuredPersonFamilyInfo();
        public InsuredPersonFamilyInfo Parent2 { get; set; } = new InsuredPersonFamilyInfo();
    }


}
