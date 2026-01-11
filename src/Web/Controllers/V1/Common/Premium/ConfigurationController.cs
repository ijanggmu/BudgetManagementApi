using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;
using Business.Common.PremiumCalculation.Service;
using Models.Common.Policy.Enum;
using System;
using System.Linq;
using Models.Common.Policy.Configuration;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class ConfigurationController(IPropertyRiskConfigurationService propertyRiskConfiguration) : BaseCommonApiController
{
    [HttpGet(nameof(GetFireConfigurations))]
    public async Task<IActionResult> GetFireConfigurations()
    {
            var pptyConfigs = await propertyRiskConfiguration.GetAllPropertyRiskConfig();
            return Ok(pptyConfigs);
    }
    [HttpGet(nameof(GetCommercialVehicleClasses))]
    public IActionResult GetCommercialVehicleClasses()
    {
        var enums = Enum.GetValues(typeof(CommercialVehicleClassEnum))
            .Cast<CommercialVehicleClassEnum>()
            .Select(e => new EnumViewModel
            {
                Value = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return Ok(enums);
    }
    [HttpGet(nameof(GetVehicleTypes))]
    public IActionResult GetVehicleTypes()
    {
        var enums = Enum.GetValues(typeof(VehicleType))
            .Cast<VehicleType>()
            .Select(e => new EnumViewModel
            {
                Value = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return Ok(enums);
    }
    [HttpGet(nameof(GetVechileEngineCapacityCategory))]
    public IActionResult GetVechileEngineCapacityCategory()
    {
        var enums = Enum.GetValues(typeof(VechileEngineCapacityCategory))
            .Cast<VechileEngineCapacityCategory>()
            .Select(e => new EnumViewModel
            {
                Value = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return Ok(enums);
    }
    [HttpGet(nameof(GetMarineType))]
    public IActionResult GetMarineType()
    {
        var enums = Enum.GetValues(typeof(MarineType))
            .Cast<MarineType>()
            .Select(e => new EnumViewModel
            {
                Value = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return Ok(enums);
    }
    [HttpGet(nameof(GetCurrency))]
    public IActionResult GetCurrency()
    {
        var enums = Enum.GetValues(typeof(Currency))
            .Cast<Currency>()
            .Select(e => new EnumViewModel
            {
                Value = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return Ok(enums);
    }
}

