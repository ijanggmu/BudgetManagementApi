using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy.Engineering
{
    public class MarineCumErectionPartialViewModel : CommonRiskTypePartialViewModel
    {
        [Required]
        public decimal BasicPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal TPLPremium { get; set; }
        [Required]
        public decimal Suminsured { get; set; }
        [Required]
        public decimal ErectionSuminsured { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
        [Required(ErrorMessage = "Days is required")]
        [Range(1, 1825, ErrorMessage = "Please enter a value less than or equal to 5 years")]
        public int PolicyPeriodInDays { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        //New Fields 
        [Required]
        public ModeOftransit ModeOfTransit { get; set; }
        public string Days { get; set; }
        [Required]
        public string CurrencyOfValue { get; set; }
        public bool IsManualExchangeRate { get; set; }
        public decimal? ExchangeRate { get; set; }
        [Range(0, 100)]
        public decimal? ToleranceRate { get; set; }

        [Range(0, 100)]
        public decimal? IncrementalCostRate { get; set; }
        [Range(0, 100)]
        public decimal? DutyRate { get; set; }
        [Required]
        public string VoyageFrom { get; set; }
        [Required]
        public string VoyageTo { get; set; }
        public string ShipConveyance { get; set; }
        public string SubjectMatterOfInsurance { get; set; }
        public string Deductible { get; set; }
        [RequiredIf(nameof(ModeOfTransit), Marine.ModeOftransit.GoodsInTransit)]
        public Marine.InlandTransitType InlandTransitType { get; set; }

        [RequiredIf(nameof(ModeOfTransit), Marine.ModeOftransit.GoodsInTransit)]
        public decimal? InlandTransitDistance { get; set; }
        public bool IsGreaterThanInlandTransitLimit { get; set; }
        public string SiteOfErection { get; set; }
        public string PeriodOfErection { get; set; }
        public string InstallationPlace { get; set; }
        public string PlantAndEquipmentErection { get; set; }
        public decimal? BodilyInjury { get; set; }
        public decimal? PropertyDamage { get; set; }
        public decimal? BodilyInjuryPerPerson { get; set; }
        public string BodilyInjuryCurrency { get; set; }
        public string PropertyDamageCurrency { get; set; }
        public string BodilyInjuryPerPersonCurrency { get; set; }
        public string TplDeductibles { get; set; }
        public decimal? BasicPremiumRate { get; set; }
        public decimal? RsmdtRate { get; set; }
        //For excess part I and II
        public decimal? MarineTransitClaimsValue { get; set; }
        [Range(0, 100)]
        public decimal? MarineTransitClaimsRate { get; set; }
        public string MarineTransitClaimsRemarks { get; set; }
        public decimal? ErectionClaimsValue { get; set; }
        [Range(0, 100)]
        public decimal? ErectionClaimsRate { get; set; }
        public string ErectionClaimsRemarks { get; set; }
        public decimal? ActOfGodClaimsValue { get; set; }
        [Range(0, 100)]
        public decimal? ActOfGodClaimsRate { get; set; }
        public string ActOfGodClaimsRemarks { get; set; }
        public decimal? TestingPeriodClaimsValue { get; set; }
        [Range(0, 100)]
        public decimal? TestingPeriodClaimsRate { get; set; }
        public string TestingPeriodClaimsRemarks { get; set; }
        public decimal? Megawatt { get; set; }
        public string Province { get; set; }

        public string District { get; set; }

        public string Municipality { get; set; }

        public string Ward { get; set; }

        public string StreetAddress { get; set; }

    }
    public enum ModeOftransit
    {
        [Display(Name = "Marine Cargo")]
        MarineCargo,
        [Display(Name = "Goods In Transit")]
        GoodsInTransit,
        [Display(Name = "Air Cargo")]
        AirCargo
    }
    public enum InlandTransitType
    {
        [Display(Name = "Inland Transit Outside Nepal")]
        OutsideNepal,
        [Display(Name = "Inland Transit Within Nepal")]
        WithinNepal
    }
}
