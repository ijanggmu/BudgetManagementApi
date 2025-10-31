using Models.Common.Policy.Calculation.Miscellaneous;
using Models.Common.Policy.Policy.Miscellaneous;

using System;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.Medical
{
    public class MedicalPolicyPartialViewModel
    {
        public List<InsuredPerson> InsuredPerson { get; set; }
        public decimal DoctorFee { get; set; }
        public decimal DoctorFeeToVisitPatientAtHome { get; set; }
        public decimal DoctorFeeLimit { get; set; }
        public decimal LimitOnCostOfMedicineMedicalAppliancesDressing { get; set; }
        public decimal LimitOnCostOfTypeOfCheckup { get; set; }
        public decimal FirstFourFeeLimit { get; set; }

        #region renewal

        /// <summary>
        /// Used in case of change in NEW/OLD renewal
        /// </summary>
        public decimal DoctorFee1 { get; set; }

        public decimal DoctorFeeToVisitPatientAtHome1 { get; set; }
        public decimal DoctorFeeLimit1 { get; set; }
        public decimal LimitOnCostOfMedicineMedicalAppliancesDressing1 { get; set; }
        public decimal LimitOnCostOfTypeOfCheckup1 { get; set; }
        public decimal FirstFourFeeLimit1 { get; set; }

        #endregion renewal

        public bool IsBasicPremiumSelected { get; set; }
        public decimal RSMDTAmount { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsRSMDTSelectedForAdditionalBenefits { get; set; }
        public decimal PremiumPerPerson { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal? ParentExtraCharge { get; set; }
        public string[] Endorsements { get; set; }
        public string[] SpecialExclusions { get; set; }
        public string[] SpecificExtensions { get; set; }
        public HIPPlans Employee { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndSpouse { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndFamily { get; set; } = new HIPPlans();
        public HIPPlans EmployeeAndParents { get; set; } = new HIPPlans();
        public HIPPlans EmployeeFamilyAndParents { get; set; } = new HIPPlans();
        public HIPPlans FlatPlan { get; set; } = new HIPPlans();
        public bool IsNew { get; set; }
        public bool IsFlatPlan { get; set; }
        public List<AdditionalBenefitProperties> AdditionalBenefits { get; set; }
        public List<MedicalBenefit> MedicalBenefits { get; set; }
        public bool IsAnnualPremiumUpdated { get; set; }
        public string AgeLimit { get; set; }
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

    public class MedicalEndorsementPartialViewModel
    {
        public List<InsuredPerson> AddedPerson { get; set; }
        public List<InsuredPerson> UpdatedPerson { get; set; }
        public List<InsuredPerson> DiscontinuedPerson { get; set; }
        public List<AdditionalBenefitProperties> AdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> AddedAdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> UpdatedAdditionalBenefits { get; set; }
        public List<AdditionalBenefitProperties> DiscontinuedAdditionalBenefits { get; set; }
        public decimal DoctorFee { get; set; }
        public decimal DoctorFeeToVisitPatientAtHome { get; set; }
        public decimal DoctorFeeLimit { get; set; }
        public decimal LimitOnCostOfMedicineMedicalAppliancesDressing { get; set; }
        public decimal LimitOnCostOfTypeOfCheckup { get; set; }
        public decimal FirstFourFeeLimit { get; set; }
    }

    public class MedicalBenefit
    {
        public string Name { get; set; }
        public string Payable { get; set; }
        public decimal Amount { get; set; }
    }
}