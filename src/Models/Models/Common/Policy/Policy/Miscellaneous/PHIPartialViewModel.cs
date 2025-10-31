using Models.Common.Policy.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class PHIInsured
    {
        [Required]
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }
        public double HSumInsured { get; set; }
        public double? CISumInsured { get; set; }
        public double HPremium { get; set; }
        public double? CIPremium { get; set; }
        public double TotalSumInsured { get; set; }
        public double TotalPremium { get; set; }
    }
    public class PHIPartialViewModel
    {
        public List<PHIBenefitsConfigurationViewModel> PHIBenefitsConfiguration { get; set; }
        [Required (ErrorMessage = "Please get the proposal no from CHI system.")]
        public string ProposalNumber { get; set; }
        public string ClientId { get; set; }
        public bool IsPreExistingDisease { get; set; }
        public string PreExistingDisease { get; set; }
        public DateTime? Dob_AD { get; set; }
        public DateTime? Dob_BS { get; set; }
        public string InsuredSingleWithCompanySince { get; set; }
        public string Father_FullName { get; set; }
        public string PartyCode { get; set; }
        public string Mother_FullName { get; set; }
        public string Remarks { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public PHIInsured InsuredPersons { get; set; } = new PHIInsured();
        public PHIInsured InsuredForPartner { get; set; } = new PHIInsured();
        public PHIInsured InsuredForChildren { get; set; } = new PHIInsured();
        public PHIInsured InsuredForChildrenOne { get; set; } = new PHIInsured();
        public decimal HSumInsured { get; set; }
        public decimal SumInsured { get; set; }
        public decimal HPremium { get; set; }
        public decimal CISumInsured { get; set; }
        public int Days { get; set; }
        public decimal CIPremium { get; set; }
        public decimal TotalSumInsuredAmount { get; set; }
        public decimal TotalPremiumAmount { get; set; }
        public decimal MedicalBenefit { get; set; }
        public string TerritorialLimit { get; set; }
        public string NomineeName { get; set; }
        public string RelationWithInsured { get; set; }
        public decimal ExcessAmount { get; set; }
        public decimal ExcessRate { get; set; }
        public string EndorsementList { get; set; }
        public string IndiaAddress { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string State { get; set; }
        public string LandMark { get; set; }
        public string DeductiblesText { get; set; }
        public string InsuredAddress { get; set; }
        public string InsuredContactNumber { get; set; }
        public string ContactId { get; set; }
        public List<PHIFamilyList> PhiFamilyList { get; set; }
        public List<EnsuredPHIFamilyList> AddedPhiFamilyList { get; set; }
        public List<EnsuredPHIFamilyList> UpdatedPhiFamilyList { get; set; }
        public List<EnsuredPHIFamilyList> DiscontinuedPhiFamilyList { get; set; }
    }
    public class PHIFamilyList   
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string Name { get; set; }
        public string Occupation { get; set; }
        public string Age { get; set; }
        public string Relation { get; set; }
        public string Gender { get; set; }
        public decimal FamilySuminsured { get; set; }
        public decimal FamilyPremium { get; set; }
        public string ClientId { get; set; }
        public string PreExistingDisease { get; set; }
        public string InsuredWithCompanySince { get; set; }
    }

    public class EnsuredPHIFamilyList:PHIFamilyList
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsExisting { get; set; }
    }
}
