using Models.Common.Policy.Calculation.Miscellaneous;
using Models.Common.Policy.Policy.Medical;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.EndorsementData
{
    public class MedicalInsuredListViewModel
    {
        public List<AdditionalBenefitProperties> AdditionalBenefits { get; set; }
        public List<InsuredPerson> InsuredPeople { get; set; }
        public string[] Endorsements { get; set; }
        public string[] SpecialExclusions { get; set; }
        public string[] SpecificExtensions { get; set; }

    }
    public class InsuredPerson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public bool IsIncludedForMedical { get; set; }
        public string PersonalInformation { get; set; }
        public string Name { get; set; }
        public string Plan { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal Premium { get; set; }
        public decimal FullPremium { get; set; }
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
        public decimal AnnualBasicPremium { get; set; }
        public decimal AnnualRSMDTPremium { get; set; }
    }


}
