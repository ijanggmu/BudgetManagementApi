using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum TractorConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "For Trailor")]
    ForTrailor = 2,

    [Display(Name = "TPL Premium As Per Horse Power")]
    TPLPremiumAsPerHorsePower = 3,

    [Display(Name = "Loading For Old Vehicle")]
    LoadingForOldVehicle = 4,

    [Display(Name = "Private Taxi")]
    PrivateTaxi = 5,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 6,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 7,

    [Display(Name = "Recovery Charge")]
    RecoveryCharge = 8,

    [Display(Name = "Per Horse Power Basic Amount")]
    PerHorsePowerBasicAmount = 9,

    [Display(Name = "No Claim Discount Rate")]
    NCDRate = 10,

    [Display(Name = "PA To Paid Driver")]
    PAToPaidDriver = 11,

    [Display(Name = "PA To Helper")]
    PAToHelper = 12,

    [Display(Name = "Riot And Strike And MD Amount Rate")]
    RiotAndStrikeAndMDAmountRate = 13,

    [Display(Name = "Terrorism Amount Rate")]
    TerrorismAmountRate = 14,

    [Display(Name = "RSMDT To Paid Driver Rate")]
    RSMDTToPaidDriverRate = 15,

    [Display(Name = "RSMDT To Helper Rate")]
    RSMDTToHelperRate = 16,

    [Display(Name = "Sum Insured Threshold Amount")]
    SumInsuredThresholdAmount = 17,

    [Display(Name = "RSMDT Sum Insured for Drivers")]
    RSMDTSumInsuredAmountForDriver = 18,

    [Display(Name = "RSMDT Sum Insured for Helper")]
    RSMDTSumInsuredAmountForHelper = 19,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 20,

    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 21,

    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 22,

    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 23,

    [Display(Name = "VAT")]
    VAT = 24,

    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 25,



    #region Govt
    [Display(Name = "Govt. TPL Premium As Per Horse Power")]
    GovtTPLPremiumAsPerHorsePower = 26,

    [Display(Name = "Govt. Loading For Old Vehicle")]
    GovtLoadingForOldVehicle = 27,

    [Display(Name = "Govt. Per Horse Power Basic Amount")]
    GovtPerHorsePowerBasicAmount = 28,

    [Display(Name = "Govt. PA To Paid Driver")]
    GovtPAToPaidDriver = 29,

    [Display(Name = "Govt. PA To Helper")]
    GovtPAToHelper = 30,

    [Display(Name = "Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 31,

    [Display(Name = "Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 32,

    [Display(Name = "Govt. RSMDT To Paid Driver Rate")]
    GovtRSMDTToPaidDriverRate = 33,

    [Display(Name = "Govt. RSMDT To Helper Rate")]
    GovtRSMDTToHelperRate = 34,

    [Display(Name = "Govt. Sum Insured Threshold Amount")]
    GovtSumInsuredThresholdAmount = 35,

    [Display(Name = "Govt. RSMDT Sum Insured for Drivers")]
    GovtRSMDTSumInsuredAmountForDriver = 36,

    [Display(Name = "Govt. RSMDT Sum Insured for Helper")]
    GovtRSMDTSumInsuredAmountForHelper = 37,

    [Display(Name = "Govt. Minimum Basic Premium Amount")]
    GovtMinimumBasicPremiumAmount = 38,

    [Display(Name = "Govt. Minimum RSMDT Amount")]
    GovtMinimumRSMDTAmount = 39,

    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 40,
    #endregion

    [Display(Name = "Per Cubic Capacity Basic Amount")]
    PerCubicCapacityBasicAmount = 41,

    [Display(Name = "Govt. Per Cubic Capacity Basic Amount")]
    GovtPerCubicCapacityBasicAmount = 42,

    [Display(Name = "Own Use Discount")]
    OwnUseDiscount = 43,

    [Display(Name = "Direct Discount for PA")]
    DirectDiscountPA = 44
}
