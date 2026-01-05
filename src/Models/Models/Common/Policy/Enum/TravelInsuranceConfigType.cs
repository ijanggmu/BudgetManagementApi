using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Enum;
public enum TravelInsuranceConfigType
{
    [Display(Name = "Stamp Duty")]
    StampDuty = 1,

    [Display(Name = "VAT")]
    VATRate = 2,

    [Display(Name = "TI Direct Discount")]
    TIDirectDiscount = 3,

    [Display(Name = "Budget Plan Rate")]
    BudgetPlanRate = 4,
}
