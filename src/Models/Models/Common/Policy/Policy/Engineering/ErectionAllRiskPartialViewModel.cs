using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Engineering
{
    public class ErectionAllRiskPartialViewModel
    {
         [Required]
        public string Currency { get; set; }
        [Required]
        public string ContractPriceCurrency { get; set; }
        [Required]
        public string MaterialSuppliedByPrincipalCurrency { get; set; }
        [Required]
        public string ConstructionPlantAndEquipmentCurrency { get; set; }
        [Required]
        public string EmployersExistingPropertyCurrency { get; set; }
        public string ClearanceOfDebrisCurrency { get; set; }
        public string CivilEngineeringWorkPermanentCurrency { get; set; }
        public string CivilEngineeringWorkTemporaryCurrency { get; set; }
        public string ArchitectsConsultantFeeCurrency { get; set; }
        public string ForeignDebrisCurrency { get; set; }
        public string OwnDebrisCurrency { get; set; }
        [Required]
        public decimal ContractPrice { get; set; }
        public decimal? BasicPremiumRate { get; set; }
        public decimal? TPLRate { get; set; }
        
        [Range(0.001,100,ErrorMessage = "Please enter a value in between 1 and 100")]
        [RegularExpression("^[0-9]+(\\.[0-9]{1,3})?$",ErrorMessage = "Cannot enter number with more than 3 decimal digits.")]
        public decimal? EscallationRate { get; set; }
        public decimal? MaterialSuppliedByPrincipal { get; set; }
        public List<CPMMaterialOfInsurance> CPMMaterialsOfInsurance { get; set; }
        public decimal? EmployersExistingProperty { get; set; }
        public decimal? ClearanceOfDebris { get; set; }
        public decimal? ForeignDebris { get; set; }
        public decimal? OwnDebris { get; set; }
        public decimal? ArchitectsConsultantFee { get; set; }
        public decimal? CivilEngineeringWorkTemporary { get; set; }
        public decimal? CivilEngineeringWorkPermanent { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsTPLSelected { get; set; }
        public decimal? BodilyInjury { get; set; }
        public decimal? PropertyDamage { get; set; }
        public decimal? BodilyInjuryPerPerson { get; set; }
        public string BodilyInjuryCurrency { get; set; }
        public string PropertyDamageCurrency { get; set; }
        public string BodilyInjuryPerPersonCurrency { get; set; }
        public string[] Endorsements { get; set; }
        public string[] Exclusions { get; set; }
        
        public decimal? Megawatt { get; set; }

        public decimal MinimumExcessAmount { get; set; }
        public decimal MinimumExcessRate { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        public decimal? SpecialDiscountRate { get; set; }

        public bool IsPremiumManual { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public string InsuredContract { get; set; }
        public string ContractNumber { get; set; }
        public string ContractPeriod { get; set; }
        public string MaintenancePeriod { get; set; }
        public string ContractSite { get; set; }
        public string ThePurchaser { get; set; }
        public decimal BasicSumInsured { get; set; }
        public decimal TPLSumInsured { get; set; }

        public string InsuredsRetainedLiabilityFirstSection { get; set; }
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
        public string InsuredsRetainedLiabilitySecondSection { get; set; }
        public string InvoiceText { get; set; }
        public bool IsInstallmentPayment { get; set; }
        public decimal BasicPremiumInstallment { get; set; }
        public decimal PoolPremiumInstallment { get; set; }
        public decimal TPLPremiumInstallment { get; set; }
        public string InstallmentDate { get; set; }
        public DateTime ExpiryDateBeforeDateExtend { get; set; }
        
        public string InsuredsRetainedLiabilityFirstSectionAOG { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> AddedCPMMaterialsOfInsurance { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> UpdatedCPMMaterialsOfInsurance { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> DiscontinuedCPMMaterialsOfInsurance { get; set; }
        
        
        [Required]
        public string PrincipalName { get; set; }
        [Required]
        public string PrincipalAddress { get; set; }
    }
}