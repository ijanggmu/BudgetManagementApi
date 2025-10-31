using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class GPARPartialViewModel:CommonAnusuchiViewModel
    {
        public List<NamedInsuredPerson> NamedInsuredPersons { get; set; }
        public List<UnNamedInsuredPerson> UnNamedInsuredPersons { get; set; }
        public List<RateViewModel> RateList { get; set; }
        public MedicalBenefitUptoBaseCampViewModel MedicalBenefitUptoBaseCampViewModel { get; set; }
        public MedicalBenefitMoreThan1900ftViewModel MedicalBenefitMoreThan1900ftViewModel { get; set; } 
        public OtherBenefitSearchNRescueViewModel OtherBenefitSearchNRescueViewModel { get; set; }
        public OtherBenefitInCaseOfDeathViewModel OtherBenefitInCaseOfDeathViewModel { get; set; }

        [Required]
        public string Expedition { get; set; }
        public bool IsEPremiumSelected { get; set; }
        public bool IsRSMDTPremiumSelected { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        public decimal? SpecialDiscountRate { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal BasicPremiumRateAbove1900Ft { get; set; }
        public decimal BasicPremiumRateUptoBaseCamp { get; set; }
        public bool IsRSMDTRateManual { get; set; }
        public decimal RSMDTRate { get; set; }
        public bool IsMedicalBenifit { get; set; }
        public decimal? ShortScaleRate { get; set; }
        public bool IsOtherBenifit { get; set; }
        public string TrekkingArea { get; set; }
        public bool IsDSelected { get; set; }
        public string ConfirmationDetails { get; set; }

        //new properties
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public string CompulsoryExcess { get; set; }
    }

    public class GPARNamedInsuredPerson : GPARInsuredPersonCommonProperties
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool ISAbove1900ft { get; set; }
    }

    public class GPARUnNamedInsuredPerson : GPARInsuredPersonCommonProperties
    {
        public int NumberofPerson { get; set; }
        public decimal Rate { get; set; }
    }

    public class GPARInsuredPersonCommonProperties
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public int SN { get; set; }
        public string Occupation { get; set; }
        public decimal SumInsured { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public decimal MedicalBenefitSumInsured { get; set; }
        public decimal Medical { get; set; }
        public decimal Pa { get; set; }
        public decimal Rsmdt { get; set; }

        // new additional fields 
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public string Designation { get; set; }
    }

    #region Medical
    public class MedicalBenefitUptoBaseCampViewModel : MedicalBenefitCommonProperties 
    {
        public int CurrentNumberOfPerson { get; set; }
    }
    public class MedicalBenefitMoreThan1900ftViewModel : MedicalBenefitCommonProperties     
    {  
        public int CurrentNumberOfPerson { get; set; }
    }
    public class MedicalBenefitCommonProperties
    {
        public int NumberofPerson { get; set; }
        public decimal SIPerPerson { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal MedicalPremiumAmount { get; set; }
        public decimal Premium { get; set; }
        public decimal MedicalPremiumAmountBeforeSpecialDiscount { get; set; }
        public decimal ShortScaleMedicalPremiumAmountBeforeSpecialDiscount { get; set; }
        public decimal MedicalSpecialDiscountAmount { get; set; }
        public decimal ShortScaleMedicalSpecialDiscountAmount { get; set; }


        // additional Fields
        public decimal? AdditionalMedicalSumInsuredPerPerson { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public decimal? TotalMedicalSumInsured { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }

    }
    #endregion


    #region Other Benefit
    public class OtherBenefitSearchNRescueViewModel : OtherBenefitCommonProperties { }
    public class OtherBenefitInCaseOfDeathViewModel : OtherBenefitCommonProperties { }
    public class OtherBenefitCommonProperties
    {
        public decimal USDRate { get; set; }
        public decimal USDSumInsuredAmount { get; set; }
        public decimal NPRSumInsuredAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal OtherPremiumAmount { get; set; }
        public decimal Premium { get; set; }
    }
    #endregion

    public class RateViewModel
    {
        public string Designation { get; set; }
        public string Status { get; set; }
        public decimal Rate { get; set; }
        public bool IsBasicPremiumRateManual { get; set; }
    }


    public class GPAREndorsementPartialViewModel: GPARPartialViewModel
    {
        public List<EndorsedNamedInsuredPerson> NamedAddedInsuredPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> UnnamedAddedInsuredPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> NamedUpdatedInsuredPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> UnnamedUpdatedInsuredPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> NamedDiscontinuedInsuredPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> UnnamedDiscontinuedInsuredPersons { get; set; }
    }

}
