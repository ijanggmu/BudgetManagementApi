using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum EVExtendedWarrantyConfigType
{
    [Display(Name = "VAT")]
    VAT = 1,
    [Display(Name = "Limit Of Liability")]
    LimitOfLiability = 2,
    [Display(Name = "Premium Rate")]
    PremiumRate = 3,
}
