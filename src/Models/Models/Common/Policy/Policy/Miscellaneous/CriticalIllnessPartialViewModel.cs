using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class CriticalIllnessPartialViewModel
    {
        public List<CriticalIllnessInsuredPerson> CriticalIllnessInsuredPerson { get; set; }
        
        public bool IsBasicPremiumSelected { get; set; }
        
        
    }

    public class CriticalIllnessInsuredPerson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int SN { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Premium { get; set; }
        public InsuredFamilyInfo Spouse { get; set; } = new InsuredFamilyInfo();
        public InsuredFamilyInfo Parent1 { get; set; } = new InsuredFamilyInfo();
        public InsuredFamilyInfo Parent2 { get; set; } = new InsuredFamilyInfo();
        public InsuredFamilyInfo Child1 { get; set; } = new InsuredFamilyInfo();
        public InsuredFamilyInfo Child2 { get; set; } = new InsuredFamilyInfo();
        public InsuredFamilyInfo Child3 { get; set; } = new InsuredFamilyInfo();
    }

    public class InsuredFamilyInfo
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Premium { get; set; }
    }

    public class CriticalIllnessEndorsementPartialViewModel : CriticalIllnessPartialViewModel
    {
        public List<CriticalIllnessInsuredPerson> AddedInsuredDetail{ get; set; }
        public List<CriticalIllnessInsuredPerson> UpdatedInsuredDetail { get; set; }
        public List<CriticalIllnessInsuredPerson> DiscontinuedInsuredDetail { get; set; }
    }
}