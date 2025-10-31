using Models.Common.Policy.Policy.Medical;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class HospitalIndemnityPlanPolicyPartialViewModel
    {
        public List<InsuredPerson> InsuredPerson { get; set; }
        public HIPPlans MaximumPerDay { get; set; } = new HIPPlans();
        /// <summary>
        /// Used for renewal when HIP is sected and needs to switch between old and new
        /// </summary>
        public HIPPlans MaximumPerDay1 { get; set; } = new HIPPlans();
        public decimal MaximumDays { get; set; }
        public HIPPlans MaximumPerSpecifiedDays { get; set; } = new HIPPlans();
        /// <summary>
        /// Used for renewal when HIP is sected and needs to switch between old and new
        /// </summary>
        public HIPPlans MaximumPerSpecifiedDays1 { get; set; } = new HIPPlans();
        public decimal Minor { get; set; }
        public decimal Intermediate { get; set; }
        public decimal Major { get; set; }

        /// <summary>
        /// Used for renewal when HIP is sected and needs to switch between old and new
        /// </summary>
        public decimal Minor1 { get; set; }
        /// <summary>
        /// Used for renewal when HIP is sected and needs to switch between old and new
        /// </summary>
        public decimal Intermediate1 { get; set; }
        /// <summary>
        /// Used for renewal when HIP is sected and needs to switch between old and new
        /// </summary>
        public decimal Major1 { get; set; }

        public decimal DoctorFee { get; set; }
        public decimal DoctorFeeToVisitPatientAtHome { get; set; }
        public decimal DoctorFeeLimit { get; set; }
        public decimal AdditionalBenefitsRSMDT { get; set; }
        public decimal AdditionalBenefitsRSMDTAmount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal RSMDT { get; set; }
        public decimal LimitOnCostOfMedicineMedicalAppliancesDressing { get; set; }
        public decimal LimitOnCostOfTypeOfCheckup { get; set; }
        public decimal FirstFourFeeLimit { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsRSMDTSelectedForAdditionalBenefits { get; set; }
        public decimal PremiumPerPerson { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal? ParentExtraCharge { get; set; }
        public bool IsHIPSelected { get; set; }
        public bool IsNew { get; set; }
        public HIPPlans Employee { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndSpouse { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndFamily { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndParents { get; set; } = new HIPPlans();
        public HIPPlans EmployeeFamilyAndParents { get; set; } = new HIPPlans();
        public HIPPlans FlatPlan { get; set; }
        public string[] Endorsements { get; set; }
        public string[] SpecialExclusions { get; set; }
        public string[] SpecificExtensions { get; set; }
        public bool IsFlatPlan { get; set; }
        public List<AdditionalBenefitProperties> AdditionalBenefits { get; set; }
        public bool IsAnnualPremiumUpdated { get; set; }

        //additional properties for unnamed childs
        public bool AddUnnamedChild { get; set; }
        public string UnnamedChildLabel { get; set; }
        public decimal UnnamedChildPremium { get; set; }
        public decimal UnnamedChildAnnualBasicPremium { get; set; }
        public decimal UnnamedChildAnnualRSMDTPremium { get; set; }
        public decimal TransactionUnnamedChildPremium { get; set; }
        public int TotalChildren { get; set; }
        public int NumberOfAddedChildren { get; set; }
        public int BalanceChildren { get; set; }
        public bool IsUnnamedChildAnnualPremiumUpdated { get; set; }

        public string BenefitFile { get; set; }
        public bool hasAddedFile { get; set; }
        public bool hasUpdatedFile { get; set; }
        public bool hasDeletedFile { get; set; }
    }
    public class HIPPlans
    {
        public decimal PlanAAmount { get; set; }
        public decimal PlanBAmount { get; set; }
        public decimal PlanCAmount { get; set; }
        public decimal PlanDAmount { get; set; }
    }

    public class AdditionalBenefitProperties
    {
        public string PrimaryId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal FullAmount { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class HIPEndorsementPartialViewModel
    {
        public List<InsuredPerson> AddedInsuredPersons { get; set; }
        public List<InsuredPerson> UpdatedInsuredPersons { get; set; }
        public List<InsuredPerson> DiscontinuedInsuredPersons { get; set; }
        public List<AdditionalBenefitProperties> AddedAdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> UpdatedAdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> DiscontinuedAdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> AdditionalBenefits { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal RSMDT { get; set; }
        public HIPPlans MaximumPerDay { get; set; } = new HIPPlans();
        public HIPPlans MaximumPerSpecifiedDays { get; set; } = new HIPPlans();
        public decimal Minor { get; set; }
        public decimal Intermediate { get; set; }
        public decimal Major { get; set; }
    }
}
