using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/configurations")]
public class AdminPremiumCalculationConfigurationController(
    IPremiumCalculationConfigurationService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all premium calculation configurations
    /// </summary>
    [HttpGet]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? portfolioAlias = null, [FromQuery] string? fiscalYear = null, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetAllConfigurationsAsync(portfolioAlias, fiscalYear, cancellationToken));
    }

    /// <summary>
    /// Get premium calculation configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetConfigurationByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Get premium calculation configuration by portfolio and fiscal year
    /// </summary>
    [HttpGet("portfolio/{portfolioAlias}/fiscal-year/{fiscalYear}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> GetByPortfolioAndFiscalYearAsync(
        string portfolioAlias, 
        string fiscalYear, 
        [FromQuery] DateTime? effectiveDate = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetConfigurationAsync(portfolioAlias, fiscalYear, effectiveDate, cancellationToken));
    }

    /// <summary>
    /// Create a new premium calculation configuration
    /// </summary>
    [HttpPost]
    [Permission(MenuPermissionConstant.PremiumConfigurationsCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateConfigurationAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Update premium calculation configuration
    /// </summary>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateConfigurationAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete premium calculation configuration (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteConfigurationAsync(id, cancellationToken));
    }

    /// <summary>
    /// Activate a premium calculation configuration
    /// </summary>
    [HttpPost("{id}/activate")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> ActivateAsync(string id, [FromBody] string fiscalYear, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.ActivateConfigurationAsync(id, fiscalYear, cancellationToken));
    }

    /// <summary>
    /// Clone configuration for a new fiscal year
    /// </summary>
    [HttpPost("{id}/clone")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> CloneAsync(string id, [FromBody] string newFiscalYear, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CloneConfigurationAsync(id, newFiscalYear, cancellationToken));
    }

    /// <summary>
    /// Validate configuration
    /// </summary>
    [HttpPost("{id}/validate")]
    [Permission(MenuPermissionConstant.PremiumConfigurationsView)]
    public async Task<IActionResult> ValidateAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.ValidateConfigurationAsync(id, cancellationToken));
    }
}

