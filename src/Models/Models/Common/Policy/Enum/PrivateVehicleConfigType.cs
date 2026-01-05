using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum PrivateVehicleConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "For First Threshold Amount")]
    ForFirstThresholdAmount = 2,

    [Display(Name = "For Above Threshold Amount")]
    ForAboveThresholdAmount = 3,

    [Display(Name = "For Trailor")]
    ForTrailor = 4,

    [Display(Name = "TPL Premium As Per CC")]
    TPLPremiumAsPerCC = 5,

    [Display(Name = "Loading For Old Vehicle")]
    LoadingForOldVehicle = 6,

    [Display(Name = "Private Taxi")]
    PrivateTaxi = 7,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 8,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 9,

    [Display(Name = "Recovery Charge")]
    RecoveryCharge = 10,

    [Display(Name = "Per CC Basic Amount")]
    PerCCBasicAmount = 11,

    [Display(Name = "No Claim Discount Rate")]
    NCDRate = 12,

    [Display(Name = "PA To Paid Driver")]
    PAToPaidDriver = 13,

    [Display(Name = "PA To Passengers")]
    PAToPassengers = 14,

    [Display(Name = "Riot And Strike And MD Amount Rate")]
    RiotAndStrikeAndMDAmountRate = 15,

    [Display(Name = "Terrorism Amount Rate")]
    TerrorismAmountRate = 16,

    [Display(Name = "RSMDT To Paid Driver Rate")]
    RSMDTToPaidDriverRate = 17,

    [Display(Name = "RSMDT To Passengers Rate")]
    RSMDTToPassengersRate = 18,

    [Display(Name = "Sum Insured Threshold Amount")]
    SumInsuredThresholdAmount = 19,

    [Display(Name = "RSMDT Sum Insured for Drivers")]
    RSMDTSumInsuredAmountForDriver = 20,

    [Display(Name = "RSMDT Sum Insured for Passengers")]
    RSMDTSumInsuredAmountForPassengers = 21,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 22,
    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 23,

    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 24,

    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 25,

    [Display(Name = "VAT")]
    VAT = 26,

    #region Govt
    [Display(Name = "Govt. Loading For Old Vehicle")]
    GovtLoadingForOldVehicle = 27,

    [Display(Name = "Govt. PA To Paid Driver")]
    GovtPAToPaidDriver = 28,

    [Display(Name = "Govt. PA To Passengers")]
    GovtPAToPassengers = 29,

    [Display(Name = "Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 30,

    [Display(Name = "Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 31,

    [Display(Name = "Govt. RSMDT To Paid Driver Rate")]
    GovtRSMDTToPaidDriverRate = 32,

    [Display(Name = "Govt. RSMDT To Passengers Rate")]
    GovtRSMDTToPassengersRate = 33,
    #endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 34,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 35,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 36,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 37,

    [Display(Name = "Govt. TPL Premium As Per CC")]
    GovtTPLPremiumAsPerCC = 38,

    [Display(Name = "Govt. Per CC Basic Amount")]
    GovtPerCCBasicAmount = 39,

    [Display(Name = "Govt. For First Threshold Amount")]
    GovtForFirstThresholdAmount = 40,

    [Display(Name = "Govt. For Above Threshold Amount")]
    GovtForAboveThresholdAmount = 41,
    [Display(Name = "Direct Discount PA")]
    DirectDiscountPA = 42,
}
