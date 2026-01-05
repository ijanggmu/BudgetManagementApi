using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum HouseholdConfigType
{
    [Display(Name = "Basic Premium Rate")]
    BasicPremiumRate = 1,

    [Display(Name = "Direct Discount")]
    DirectDiscount = 2,

    [Display(Name = "RSMD Rate")]
    RSMDRate = 3,

    [Display(Name = "Terrorism Rate")]
    TerrorismRate = 4,
    [Display(Name = "VAT")]
    VAT = 5,
    [Display(Name = "Second Class Basic Premium Rate")]
    SecondClassBasicPremiumRate = 6,
    [Display(Name = "Minimum Basic Premium Amount")]
    MinimunBasicPremiumAmount = 7,
    [Display(Name = "Minimum RSMDT Amount")]
    MinimunRSMDTAmount = 8,
    [Display(Name = "Government Subsidy Rate")]
    SubsidyRate = 9
}
