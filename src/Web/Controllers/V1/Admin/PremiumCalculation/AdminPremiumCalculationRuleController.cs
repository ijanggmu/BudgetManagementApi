using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/rules")]
public class AdminPremiumCalculationRuleController(
    IPremiumCalculationRuleService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all rules for a configuration
    /// </summary>
    [HttpGet("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetByConfigurationAsync(string configurationId, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetRulesByConfigurationIdAsync(configurationId, cancellationToken));
    }

    /// <summary>
    /// Get rule by ID
    /// </summary>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetRuleByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Create a new rule
    /// </summary>
    [HttpPost("configuration/{configurationId}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsCreate)]
    public async Task<IActionResult> CreateAsync(string configurationId, [FromBody] CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateRuleAsync(configurationId, dto, cancellationToken));
    }

    /// <summary>
    /// Update a rule
    /// </summary>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateRuleAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete a rule
    /// </summary>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteRuleAsync(id, cancellationToken));
    }
}

