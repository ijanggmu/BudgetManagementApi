using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class TrekkingRescuePartialViewModel:CommonRiskTypePartialViewModel
    {
        public List<TRNamedInsuredPerson> NamedInsuredPersons { get; set; }
        public List<TRUnNamedInsuredPerson> UnNamedInsuredPersons { get; set; }
        public List<RateViewModel> RateList { get; set; }
        public TRMedicalBenefitUpto19000ftViewModel TRMedicalBenefitUpto19000ftViewModel { get; set; }
        public TRMedicalBenefitMoreThan19000ftViewModel TRMedicalBenefitMoreThan19000ftViewModel { get; set; } 
        public TROtherBenefitSearchNRescueViewModel OtherBenefitSearchNRescueViewModel { get; set; }
        public TROtherBenefitInCaseOfDeathViewModel OtherBenefitInCaseOfDeathViewModel { get; set; }

        [Required]
        public string Expedition { get; set; }
        public bool IsEPremiumSelected { get; set; }
        public bool IsRSMDTPremiumSelected { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        public bool ProvideCorporateDiscount { get; set; }
        public decimal? SpecialDiscountRate { get; set; }
        public decimal CorporateDiscountRate { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal AdministrativeStaffRate { get; set; }
        public decimal NamedAbove19000FtRate { get; set; }
        public decimal UnNamedAbove19000FtRate { get; set; }        
        public decimal NamedBelow19000FtRate { get; set; }
        public decimal UnNamedBelow19000FtRate { get; set; }
        public bool IsRSMDTRateManual { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal ShortScaleRate{ get; set; }
        public bool IsMedicalBenifit { get; set; }
        public bool IsOtherBenifit { get; set; }
        public string TrekkingArea { get; set; }
        
        // Print Page Schedule 
        public string PersonalAccidentInsurance { get; set; }
        public string AccidentalMedicalExpenses { get; set; }
        public string SearchAndRescueOperation { get; set; }
        public string RepatriationOFDeath { get; set; }
        public bool IsDSelected { get; set; }
        public string ConfirmationDetails { get; set; }

        //new properties
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string CompulsoryExcess { get; set; }
        public string InsuredCoveredByThePolicy { get; set; }
        public string ProposerName { get; set; }
        public string ProposerAddress { get; set; }
        public string BenefitCovered { get; set; }
        //old fields
        public decimal BasicPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal Suminsured { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
    }

    public class TRNamedInsuredPerson : TRInsuredPersonCommonProperties
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TRUnNamedInsuredPerson : TRInsuredPersonCommonProperties
    {
        public int NumberofPerson { get; set; }
        public decimal MedicalSumInsuredLimitPI { get; set; }
        public decimal SumInsuredPI { get; set; }
        public decimal AdditionalMedicalAmountPI { get; set; }

    }

    public class TRInsuredPersonCommonProperties
    {
        public decimal Rate { get; set; }
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
        public bool IsAdded { get; set; }
        public decimal MedicalSumInsuredLimit { get; set; }
        public decimal AdditionalMedicalSumInsured { get; set; }
        public decimal MedicalSumInsuredPI { get; set; }
        public decimal TotalMedicalSumInsured { get; set; }
        public decimal Pa { get; set; }
        public decimal Medical { get; set; }
        public decimal Rsmdt { get; set; }
        public string Designation { get; set; }
        public int ChangeInCount { get; set; }
        public bool ISAbove19000Ft { get; set; }
        public bool ISAdministrativeStaff { get; set; }
    }

    #region Medical
    public class TRMedicalBenefitUpto19000ftViewModel : TRMedicalBenefitCommonProperties 
    {
        public int CurrentNumberOfPerson { get; set; }
    }
    public class TRMedicalBenefitMoreThan19000ftViewModel : TRMedicalBenefitCommonProperties     
    {  
        public int CurrentNumberOfPerson { get; set; }
    }
    public class TRMedicalBenefitCommonProperties
    {
        public int NumberofPerson { get; set; }
        public decimal SIPerPerson { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal MedicalPremiumAmount { get; set; }
        public decimal Premium { get; set; }
        public decimal MedicalPremiumAmountBeforeSpecialDiscount { get; set; }

        // additional Fields
        public decimal? AdditionalMedicalSumInsuredPerPerson { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public decimal? TotalMedicalSumInsured { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }

    }
    #endregion


    #region Other Benefit
    public class TROtherBenefitSearchNRescueViewModel : TROtherBenefitCommonProperties { }
    public class TROtherBenefitInCaseOfDeathViewModel : TROtherBenefitCommonProperties { }
    public class TROtherBenefitCommonProperties
    {
        public decimal USDRate { get; set; }
        public decimal USDSumInsuredAmount { get; set; }
        public decimal NPRSumInsuredAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal OtherPremiumAmount { get; set; }
        public decimal Premium { get; set; }
    }
    #endregion

    public class TRRateViewModel
    {
        public string Designation { get; set; }
        public string Status { get; set; }
        public decimal Rate { get; set; }
        public bool IsBasicPremiumRateManual { get; set; }
    }


    public class TREndorsementPartialViewModel: TrekkingRescuePartialViewModel
    {
        public List<TRNamedInsuredPerson> NamedAddedInsuredPersons { get; set; }
        public List<TRUnNamedInsuredPerson> UnnamedAddedInsuredPersons { get; set; }
        public List<TRNamedInsuredPerson> NamedUpdatedInsuredPersons { get; set; }
        public List<TRUnNamedInsuredPerson> UnnamedUpdatedInsuredPersons { get; set; }
        public List<TRNamedInsuredPerson> NamedDiscontinuedInsuredPersons { get; set; }
        public List<TRUnNamedInsuredPerson> UnnamedDiscontinuedInsuredPersons { get; set; }
    }
}