using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum ElectricMotorcycleConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 2,

    [Display(Name = "Loading Rate")]
    LoadingRate = 3,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 4,

    [Display(Name = "Per KW BasicAmount")]
    PerKWBasicAmount = 5,

    [Display(Name = "NCD Rate")]
    NCDRate = 6,

    [Display(Name = "Riot, Strike, MD Amount Rate")]
    RiotAndStrikeAndMDAmountRate = 7,

    [Display(Name = "Terrorism Amount Rate")]
    TerrorismAmountRate = 8,

    [Display(Name = "PA To Rider and 1 Pillion Rider Rate")]
    PAToRiderandonePillionRiderRate = 9,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 10,

    [Display(Name = "Minimum Basic Premium Amount")]
    MinimumBasicPremiumAmount = 11,

    [Display(Name = "Minimum RSMDT Amount")]
    MinimumRSMDTAmount = 12,

    [Display(Name = "PA To Rider and 1 Pillion Rider Amount")]
    PAToRiderandonePillionRiderAmount = 13,

    [Display(Name = "Compulsory Excess")]
    CompulsoryExcess = 14,

    [Display(Name = "Minimum Period For Depreciation (months)")]
    MinimumPeriodForDepreciation = 15,

    [Display(Name = "Eco friendly Discount Rate")]
    EcoFriendlyDiscountRate = 16,

    [Display(Name = "VAT")]
    VAT = 17,

    #region GOVT
    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 18,

    [Display(Name = "Govt. Loading Rate")]
    GovtLoadingRate = 19,

    [Display(Name = "Govt. Riot And Strike And MD Amount Rate")]
    GovtRiotAndStrikeAndMDAmountRate = 20,

    [Display(Name = "Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 21,

    [Display(Name = "Govt. PA To Rider and 1 Pillion Rider Rate")]
    GovtPAToRiderandonePillionRiderRate = 22,

    [Display(Name = "Govt. Eco friendly Discount Rate")]
    GovtEcoFriendlyDiscountRate = 23,
    #endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 24,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 25,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 26,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 27,
}
