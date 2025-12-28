using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum MotorCycleConfigType
{
    [Display(Name = "Depreciation Rate")]
    DepreciationRate = 1,

    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 2,

    [Display(Name = "Loading Rate")]
    LoadingRate = 3,

    [Display(Name = "Voluntary Excess Rate")]
    VoluntaryExcessRate = 4,

    [Display(Name = "Per CC Basic Amount")]
    PerCCBasicAmount = 5,

    [Display(Name = "No Claim Discount Rate")]
    NCDRate = 6,

    [Display(Name = "Riot, Strike, MDAmountRate")]
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

    [Display(Name = "Agent Commission")]
    AgentCommission = 16,

    [Display(Name = "VAT")]
    VAT = 17,

    #region Govt
    [Display(Name = "Govt. Basic Premium Rate")]
    GovtBasicPremiumRate = 18,

    [Display(Name = "Govt. Loading Rate")]
    GovtLoadingRate = 19,

    [Display(Name = "Govt. Riot, Strike, MDAmountRate")]
    GovtRiotAndStrikeAndMDAmountRate = 20,

    [Display(Name = "Govt. Terrorism Amount Rate")]
    GovtTerrorismAmountRate = 21,

    [Display(Name = "Govt. PA To Rider and 1 Pillion Rider Rate")]
    GovtPAToRiderandonePillionRiderRate = 22,
    #endregion

    [Display(Name = "Maximum Sum Insured")]
    MaximumSI = 23,
    [Display(Name = "Per Person Medical Sum Insured")]
    PerPersonMedicalSI = 24,
    [Display(Name = "Per Person Death Sum Insured")]
    PerPersonDeathSI = 25,
    [Display(Name = "Property Damage Sum Insured")]
    PropertyDamageSI = 26,

    [Display(Name = "Govt. Per CC Basic Amount")]
    GovtPerCCBasicAmount = 27,
}
