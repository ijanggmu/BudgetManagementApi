using System;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.Fire
{
    public class FirePolicyPartialViewModel
    {
        public List<SubjectMatterOfInsurance> SubjectMatterOfInsurance { get; set; }

        //Section 5
        public DateTime PeriodOfInsuranceFrom { get; set; }
        public DateTime PeriodOfInsuranceTo { get; set; }
        public string InsuranceType { get; set; }
        public string NatureOfProperty { get; set; }
        public bool IsAgentInvolved { get; set; }
        
        public decimal? Megawatt { get; set; }

        // Risk Types
        public bool IsRSMDT { get; set; }

        //Endorsement
        public List<SubjectMatterOfInsurance> AddedSubjectMatterOfInsurance { get; set; }
        public List<SubjectMatterOfInsurance> UpdatedSubjectMatterOfInsurance { get; set; }
        public List<SubjectMatterOfInsurance> DiscontinuedSubjectMatterOfInsurance { get; set; }

        //Lockdown and Subsidy
        public bool ProvideLockdownDiscount { get; set; }
        public List<LockdownDiscountViewModel> LockdownDiscountDetail { get; set; }
        public bool ProvideSubsidy { get; set; }
        public decimal SubsidyRate { get; set; }
        public string SubsidyClassID { get; set; }
        public string SubsidyClass { get; set; }
        public decimal SubsidyLimit { get; set; }
    }
}
