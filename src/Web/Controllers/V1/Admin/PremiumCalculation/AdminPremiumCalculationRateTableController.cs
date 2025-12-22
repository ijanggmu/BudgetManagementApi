using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/rate-tables")]
public class AdminPremiumCalculationRateTableController(
    IPremiumCalculationRateTableService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all rate tables for a configuration
    /// </summary>
    [HttpGet("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetByConfigurationAsync(string configurationId, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetRateTablesByConfigurationIdAsync(configurationId, cancellationToken));
    }

    /// <summary>
    /// Get rate table by ID
    /// </summary>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetRateTableByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Create a new rate table
    /// </summary>
    [HttpPost("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsCreate)]
    public async Task<IActionResult> CreateAsync(string configurationId, [FromBody] CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateRateTableAsync(configurationId, dto, cancellationToken));
    }

    /// <summary>
    /// Update a rate table
    /// </summary>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateRateTableAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete a rate table
    /// </summary>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteRateTableAsync(id, cancellationToken));
    }
}

