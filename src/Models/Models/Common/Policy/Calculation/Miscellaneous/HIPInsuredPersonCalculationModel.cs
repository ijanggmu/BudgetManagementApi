using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class HIPInsuredPersonCalculationModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public int SN { get; set; }
        public bool IsIncludedForMedical { get; set; }
        public string PersonalInformation { get; set; }
        public string OfficialInformation { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Plan { get; set; }
        public string EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public InsuredPersonFamilyInfo Spouse { get; set; }
        public InsuredPersonFamilyInfo Child1 { get; set; }
        public InsuredPersonFamilyInfo Child2 { get; set; }
        public InsuredPersonFamilyInfo Child3 { get; set; }
        public InsuredPersonFamilyInfo Child4 { get; set; }
        public InsuredPersonFamilyInfo Child5 { get; set; }
        public InsuredPersonFamilyInfo Child6 { get; set; }
        public InsuredPersonFamilyInfo Child7 { get; set; }
        public InsuredPersonFamilyInfo Child8 { get; set; }
        public InsuredPersonFamilyInfo Child9 { get; set; }
        public InsuredPersonFamilyInfo Child10 { get; set; }
        public InsuredPersonFamilyInfo Parent1 { get; set; }
        public InsuredPersonFamilyInfo Parent2 { get; set; }
        public bool IsActive { get; set; }
        public decimal Premium { get; set; }
        public decimal FullPremium { get; set; }
        public bool IsNew { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public decimal AnnualBasicPremium { get; set; }
        public decimal AnnualRSMDTPremium { get; set; }
    }
}
