using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class DepositorAndLoaneeOfBankPartialViewModel
    {
        public decimal GroupPersonalAccidentInsuranceSIPerPerson { get; set; }
        public decimal MedicalInsuranceSIPerPerson { get; set; }
        public decimal CriticalIllnessInsuranceSIPerPerson { get; set; }
        public decimal NumberOfPersonInsured { get; set; }
        public decimal TotalGroupPersonalAccidentInsuranceSI { get; set; }
        public decimal TotalMedicalInsuranceSI { get; set; }
        public decimal TotalCriticalIllnessInsuranceSI { get; set; }
        public decimal? GroupPersonalAccidentInsuranceRate { get; set; }
        public decimal? MedicalInsuranceRate { get; set; }
        public decimal? CriticalIllnessInsuranceRate { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IncludeGPAInsurance { get; set; }
        public bool IncludeMedicalInsurance { get; set; }
        public bool IncludeCriticalIllnessInsurance { get; set; }
        public bool IsManualPremiumSelected { get; set; }
        public string ConfirmationDetails { get; set; }
        public decimal GroupDiscountRate { get; set; }
        public decimal RSMDTRate { get; set; }

        //changes DNL
        public List<NamedDNLInsuredPerson> NamedInsuredPersons { get; set; }     
        public List<UnnamedDNLInsuredPerson> UnnamedInsuredPersons { get; set; }
        public List<EndorsedNamedDNLInsuredPerson> EndorsedNamedInsuredPersons { get; set; }
        public List<EndorsedUnnamedDNLInsuredPerson> EndorsedUnnamedInsuredPersons { get; set; }
        public List<EndorsedNamedDNLInsuredPerson> AddedNamedInsuredPersons { get; set; }
        public List<EndorsedNamedDNLInsuredPerson> UpdatedNamedInsuredPersons { get; set; }
        public List<EndorsedNamedDNLInsuredPerson> DiscontinuedNamedInsuredPersons { get; set; }
        public List<EndorsedUnnamedDNLInsuredPerson> AddedUnnamedInsuredPersons { get; set; }
        public List<EndorsedUnnamedDNLInsuredPerson> UpdatedUnnamedInsuredPersons { get; set; }
        public List<EndorsedUnnamedDNLInsuredPerson> DiscontinuedUnnamedInsuredPersons { get; set; }
    }

    public class NamedDNLInsuredPerson : CommonFields
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }

        public string InsuredId { get; set; }
    }
    public class UnnamedDNLInsuredPerson : CommonFields
    {
        public int NumberOfPerson { get; set; }
        public string GroupName { get; set; }
        public decimal? GpaInsuranceSumInsuredPerPerson { get; set; }
        public decimal? MedicalInsuranceSumInsuredPerPerson { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsuredPerPerson { get; set; }
    }

    public class EndorsedNamedDNLInsuredPerson : EndorsedCommonFields
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }
        public string InsuredId { get; set; }
    }

    public class EndorsedUnnamedDNLInsuredPerson : EndorsedCommonFields
    {
        public int NumberOfPerson { get; set; }
        public int ChangeInCount { get; set; }
        public string GroupName { get; set; }
        public decimal? GpaInsuranceSumInsuredPerPerson { get; set; }
        public decimal? MedicalInsuranceSumInsuredPerPerson { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsuredPerPerson { get; set; }

    }

    public class CommonFields
    {
        public string Id { get; set; }
        public string PrimaryId { get; set; }
        public bool IsGpaInsuranceSelected { get; set; }
        public bool IsMedicalInsuranceSelected { get; set; }
        public bool IsCriticalIllnessInsuranceSelected { get; set; }
        public decimal? GpaInsuranceCoinsuranceSumInsured { get; set; }
        public decimal? GpaInsuranceSumInsured { get; set; }
        public decimal? MedicalInsuranceSumInsured { get; set; }
        public decimal? MedicalInsuranceCoinsuranceSumInsured { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsured { get; set; }
        public decimal? CriticalIllnessInsuranceCoinsuranceSumInsured { get; set; }
        public decimal GpaInsurancePremium { get; set; }
        public decimal MedicalInsurancePremium { get; set; }
        public decimal CriticalIllnessInsurancePremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public bool IsUpdatedUnnamed { get; set; }
    }

    public class EndorsedCommonFields : CommonFields
    {
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
