using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum ElectricCommercialVehicleConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 2,

    [Display(Name = "Loading Rate")]
    LoadingRate = 3,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 4,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 5,

    [Display(Name = "Recovery Charge")]
    RecoveryCharge = 6,

    [Display(Name = "Per CC Basic Amount")]
    PerCCBasicAmount = 7,

    [Display(Name = "No Claim Discount Rate")]
    NCDRate = 8,

    [Display(Name = "PA To Paid Driver")]
    PAToPaidDriver = 9,

    [Display(Name = "PA To Passengers")]
    PAToPassengers = 10,

    [Display(Name = "Riot And Strike And MD Amount Rate")]
    RiotAndStrikeAndMDAmountRate = 11,

    [Display(Name = "Terrorism Amount Rate")]
    TerrorismAmountRate = 12,

    [Display(Name = "RSMDT To Paid Driver Rate")]
    RSMDTToPaidDriverRate = 13,

    [Display(Name = "RSMDT To Passengers Rate")]
    RSMDTToPassengersRate = 14,

    [Display(Name = "RSMDT Sum Insured Amount For Paid Driver")]
    RSMDTSumInsuredAmountForPaidDriver = 15,

    [Display(Name = "RSMDT Sum Insured Amount For Passengers")]
    RSMDTSumInsuredAmountForPassengers = 16,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 17,
    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 18,
    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 19,
    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 20,

    [Display(Name = "VAT")]
    VAT = 21,

    #region Govt
    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 22,
    [Display(Name = "Govt. Loading Rate")]
    GovtLoadingRate = 23,
    [Display(Name = " Govt. PA To Paid Driver")]
    GovtPAToPaidDriver = 24,
    [Display(Name = " Govt. PA To Passengers")]
    GovtPAToPassengers = 25,
    [Display(Name = " Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 26,
    [Display(Name = " Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 27,
    [Display(Name = " Govt. RSMDT To Paid Driver Rate")]
    GovtRSMDTToPaidDriverRate = 28,
    [Display(Name = " Govt. RSMDT To Passengers Rate")]
    GovtRSMDTToPassengersRate = 29,
    #endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 30,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 31,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 32,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 33,
    [Display(Name = "Govt. Per CC Basic Amount")]
    GovtPerCCBasicAmount = 34,
    [Display(Name = "Private Use Discount")]
    PrivateUseDiscount = 35,
    [Display(Name = "Direct Discount PA")]
    DirectDiscountPA = 36,
}
