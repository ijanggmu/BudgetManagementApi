using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum InternationalTravelInsuranceConfigType
{
    [Display(Name = "Stamp Duty")]
    StampDuty = 1,

    [Display(Name = "VAT")]
    VATRate = 2,

    [Display(Name = "Plan 1 (USD)")]
    PlanOne = 3,

    [Display(Name = "Plan 2 (USD)")]
    PlanTwo = 4,

    [Display(Name = "COVID 19 Coverage")]
    COVIDCoverage = 5,
}
