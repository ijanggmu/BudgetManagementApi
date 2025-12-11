using System.Collections.Generic;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/parameters")]
public class AdminPremiumCalculationParameterController(
    IPremiumCalculationParameterService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all parameters for a configuration
    /// </summary>
    [HttpGet("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetByConfigurationAsync(string configurationId)
    {
        return HandleResult(await service.GetParametersByConfigurationIdAsync(configurationId));
    }

    /// <summary>
    /// Get parameter by ID
    /// </summary>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetParameterByIdAsync(id));
    }

    /// <summary>
    /// Create a new parameter
    /// </summary>
    [HttpPost("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsCreate)]
    public async Task<IActionResult> CreateAsync(string configurationId, [FromBody] CreatePremiumCalculationParameterDto dto)
    {
        return HandleResult(await service.CreateParameterAsync(configurationId, dto));
    }

    /// <summary>
    /// Update a parameter
    /// </summary>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreatePremiumCalculationParameterDto dto)
    {
        return HandleResult(await service.UpdateParameterAsync(id, dto));
    }

    /// <summary>
    /// Delete a parameter
    /// </summary>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsDelete)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await service.DeleteParameterAsync(id));
    }

    /// <summary>
    /// Bulk update parameters for a configuration
    /// </summary>
    [HttpPost("configuration/{configurationId}/bulk")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> BulkUpdateAsync(string configurationId, [FromBody] List<CreatePremiumCalculationParameterDto> parameters)
    {
        return HandleResult(await service.BulkUpdateParametersAsync(configurationId, parameters));
    }
}

