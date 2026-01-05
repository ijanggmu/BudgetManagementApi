using System.Threading.Tasks;
using Business.Common.PolicyCalculator;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;
using SharedKernel.Constant.Permission;
using Business.Common.PremiumCalculation.Service;
using Data.Entities.Calculation;
using Models.Common.Policy.Policy.Fire;
using System.Collections.Generic;
using System.IO;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class ConfigurationController(IPropertyRiskConfigurationService propertyRiskConfiguration) : BaseCommonApiController
{
    [HttpGet(nameof(GetFireConfigurations))]
    public async Task<IActionResult> GetFireConfigurations()
    {
            var pptyConfigs = await propertyRiskConfiguration.GetAllPropertyRiskConfig();
            return Ok(pptyConfigs);
    }

}

