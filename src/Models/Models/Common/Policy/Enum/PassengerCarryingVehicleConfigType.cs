using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum PassengerCarryingVehicleConfigType
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

    [Display(Name = "Per Basic Seat Capacity")]
    PerBasicSeatCapacity = 10,

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

    [Display(Name = "RSMDT Sum Insured Amount For Paid Driver")]
    RSMDTSumInsuredAmountForPaidDriver = 20,

    [Display(Name = "RSMDT Sum Insured Amount For Helper")]
    RSMDTSumInsuredAmountForHelper = 21,

    [Display(Name = "RSMDT Sum Insured Amount For Passengers")]
    RSMDTSumInsuredAmountForPassengers = 22,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 23,
    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 24,
    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 25,
    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 26,
    [Display(Name = "VAT")]
    VAT = 27,

    #region Govt
    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 28,
    [Display(Name = "Govt. Loading Rate")]
    GovtLoadingRate = 29,
    [Display(Name = "Govt. As Per Seat Capacity")]
    GovtAsPerSeatCapacity = 30,
    [Display(Name = "Govt. Per Basic Seat Capacity")]
    GovtPerBasicSeatCapacity = 31,
    [Display(Name = " Govt. PA To Paid Driver")]
    GovtPAToPaidDriver = 32,
    [Display(Name = " Govt. PA To Helper")]
    GovtPAToHelper = 33,
    [Display(Name = " Govt. PA To Passengers")]
    GovtPAToPassengers = 34,
    [Display(Name = " Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 35,
    [Display(Name = " Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 36,
    [Display(Name = " Govt. RSMDT To Paid Driver Rate")]
    GovtRSMDTToPaidDriverRate = 37,
    [Display(Name = " Govt. RSMDT To Helper Rate")]
    GovtRSMDTToHelperRate = 38,
    [Display(Name = " Govt. RSMDT To Passengers Rate")]
    GovtRSMDTToPassengersRate = 39,
    #endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 40,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 41,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 42,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 43,
    [Display(Name = "Direct Discount PA")]
    DirectDiscountPA = 44,
}
