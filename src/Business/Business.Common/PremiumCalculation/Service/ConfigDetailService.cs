using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy;

namespace Business.Common.PremiumCalculation.Service;
public static class ConfigDetailService
{
    public static decimal GetCubicCapacityFromEngineCapacity(VechileEngineCapacityCategory engineCapacity)
    {
        return engineCapacity switch
        {
            VechileEngineCapacityCategory.Under1000 => 999,
            VechileEngineCapacityCategory.From1000To1500 => 1500,
            VechileEngineCapacityCategory.Above1500 => 1501,
            _ => throw new ArgumentOutOfRangeException(nameof(engineCapacity))
        };
    }
    public static decimal GetKiloWattFromEngineCapacity(VechileEngineCapacityCategory engineCapacity)
    {
        return engineCapacity switch
        {
            VechileEngineCapacityCategory.Under1000 => 999,
            VechileEngineCapacityCategory.From1000To1500 => 1500,
            VechileEngineCapacityCategory.Above1500 => 1501,
            _ => 1000
        };
    }
    public static VechileEngineCapacityCategory GetCategoryFromCubicCapacity(decimal cubicCapacity)
    {
        if (cubicCapacity < 1000)
            return VechileEngineCapacityCategory.Under1000;
        else if (cubicCapacity >= 1000 && cubicCapacity <= 1500)
            return VechileEngineCapacityCategory.From1000To1500;
        else
            return VechileEngineCapacityCategory.Above1500;
    }

    // Convert kilowatt power to engine capacity category
    public static KilowattRange GetCategoryFromKilowatts(decimal kilowatts)
    {
        if (kilowatts < 799)
            return KilowattRange.Range0To799;
        else if (kilowatts >= 799 && kilowatts <= 1199)
            return KilowattRange.Range799To1199;
        else
            return KilowattRange.Range1200Plus;
    }
    public static NoClaimDiscountType MapToNoClaimDiscountType(int years)
    {
        return years switch
        {
            0 => NoClaimDiscountType.Year0_0Percent,
            1 => NoClaimDiscountType.Year1_15Percent,
            2 => NoClaimDiscountType.Year2_25Percent,
            >= 3 => NoClaimDiscountType.Year3OrMore_35Percent,
            _ => NoClaimDiscountType.Year0_0Percent
        };
    }

}
