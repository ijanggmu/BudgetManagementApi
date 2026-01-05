using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum MarineConfigType
{
    [Display(Name = "Discount On Air Transit")]
    DiscountOnAirTransit = 1,
    [Display(Name = "Discount On Inland Transit Within Nepal")]
    DiscountOnInlandTransitWithinNepal = 2,
    [Display(Name = "Discount On Inland Transit Outside Nepal")]
    DiscountOnInlandTransitOutsideNepal = 3,
    [Display(Name = "Container Discount")]
    ContainerDiscount = 4,
    [Display(Name = "Discount On Letter of Credit")]
    DiscountOnLetterOfCredit = 5,
    [Display(Name = "Discount By Declared Policy")]
    DiscountByDeclaredPolicy = 6,
    [Display(Name = "Non Delivery")]
    NonDelivery = 7,
    [Display(Name = "Water Damage")]
    WaterDamage = 8,
    [Display(Name = "Theft/Prefetch/Non Delivery")]
    TPND = 9,
    [Display(Name = "SRCC For Inland Only")]
    SRCCForInlandOnly = 10,
    [Display(Name = "SRCC By Ship")]
    SRCCIncludingByShip = 11,
    [Display(Name = "SRCC By Air")]
    SRCCByAir = 12,
    [Display(Name = "Direct Discount")]
    DirectDiscount = 13,
    [Display(Name = "VAT")]
    VAT = 14,
    [Display(Name = "Minimum Gross Premium")]
    MinimumGrossPremium = 15,
    [Display(Name = "Marine Extension Risk")]
    MarineExtensionRisk = 16,
    [Display(Name = "Installment Limit")]
    InstallmentLimit = 17
}
