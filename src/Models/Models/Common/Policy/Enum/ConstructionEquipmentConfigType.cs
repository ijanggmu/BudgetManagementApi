using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum ConstructionEquipmentConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 2,

    [Display(Name = "As Per Seat Capacity")]
    AsPerSeatCapacity = 3,

    [Display(Name = "For Trailor")]
    ForTrailor = 4,

    [Display(Name = "Loading Rate")]
    LoadingRate = 5,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 6,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 7,

    [Display(Name = "Private Use Discount")]
    PrivateUseDiscount = 8,

    [Display(Name = "Recovery Charge")]
    RecoveryCharge = 9,

    [Display(Name = "Per Carrying Capacity in tons")]
    PerCarryingCapacityInTon = 10,

    [Display(Name = "No Claim Discount Rate")]
    NCDRate = 11,

    [Display(Name = "PA To Paid Driver")]
    PAToPaidDriver = 12,

    [Display(Name = "PA To Helper")]
    PAToHelper = 13,

    [Display(Name = "PA To Passengers")]
    PAToPassengers = 14,

    [Display(Name = "Riot And Strike And MD Amount Rate")]
    RiotAndStrikeAndMDAmountRate = 15,

    [Display(Name = "Terrorism Amount Rate")]
    TerrorismAmountRate = 16,

    [Display(Name = "RSMDT To Paid Driver Rate")]
    RSMDTToPaidDriverRate = 17,

    [Display(Name = "RSMDT To Helper Rate")]
    RSMDTToHelperRate = 18,

    [Display(Name = "RSMDT To Passengers Rate")]
    RSMDTToPassengersRate = 19,

    [Display(Name = "Minimum Ton Excess Rate Amount")]
    MinimumTonExcessRateAmount = 20,

    [Display(Name = "Minimum Ton Excess Value")]
    MinimumTonExcessValue = 21,

    [Display(Name = "RSMDT SI Amount For Paid Driver")]
    RSMDTSumInsuredAmountForPaidDriver = 22,

    [Display(Name = "RSMDT SI Amount For Helper")]
    RSMDTSumInsuredAmountForHelper = 23,

    [Display(Name = "RSMDT SI Amount For Passengers")]
    RSMDTSumInsuredAmountForPassengers = 24,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 25,

    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 26,

    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 27,

    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 28,

    [Display(Name = "VAT")]
    VAT = 29,

    //#region Govt
    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 30,

    [Display(Name = "Govt. As Per Seat Capacity")]
    GovtAsPerSeatCapacity = 31,

    [Display(Name = "Govt. Loading Rate")]
    GovtLoadingRate = 32,

    [Display(Name = "Govt. Per Carrying Capacity in tons")]
    GovtPerCarryingCapacityInTon = 33,

    [Display(Name = "Govt. PA To Paid Driver")]
    GovtPAToPaidDriver = 34,

    [Display(Name = "Govt. PA To Helper")]
    GovtPAToHelper = 35,

    [Display(Name = "Govt. PA To Passengers")]
    GovtPAToPassengers = 36,

    [Display(Name = "Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 37,

    [Display(Name = "Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 38,

    [Display(Name = "Govt. RSMDT To Paid Driver Rate")]
    GovtRSMDTToPaidDriverRate = 39,

    [Display(Name = "Govt. RSMDT To Helper Rate")]
    GovtRSMDTToHelperRate = 40,

    [Display(Name = "Govt. RSMDT To Passengers Rate")]
    GovtRSMDTToPassengersRate = 41,

    [Display(Name = "Govt. Minimum Ton Excess Rate Amount")]
    GovtMinimumTonExcessRateAmount = 42,

    [Display(Name = "Govt. Minimum Ton Excess Value")]
    GovtMinimumTonExcessValue = 43,

    [Display(Name = "Govt. RSMDT SI Amount For Paid Driver")]
    GovtRSMDTSumInsuredAmountForPaidDriver = 44,

    [Display(Name = "Govt. RSMDT SI Amount For Helper")]
    GovtRSMDTSumInsuredAmountForHelper = 45,

    [Display(Name = "Govt. RSMDT SI Amount For Passengers")]
    GovtRSMDTSumInsuredAmountForPassengers = 46,

    [Display(Name = "Govt. Minimum Basic Premium Amount")]
    GovtMinimumBasicPremiumAmount = 47,

    [Display(Name = "Govt. Minimum RSMDT Amount")]
    GovtMinimumRSMDTAmount = 48,

    //#endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 49,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 50,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 51,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 52,
    [Display(Name = "TPL For Heavy Equipments")]
    TPLForHeavyEquipments = 53,

    [Display(Name = "Govt TPL For Heavy Equipments")]
    GovtTPLForHeavyEquipments = 54,

    [Display(Name = "As Per Seat Capacity For Heavy Equipments")]
    AsPerSeatCapacityForHeavyEquipments = 55,

    [Display(Name = "Govt As Per Seat Capacity For Heavy Equipments")]
    GovtAsPerSeatCapacityForHeavyEquipments = 56,

    [Display(Name = "Direct Discount for PA")]
    DirectDiscountPA = 57,
}
