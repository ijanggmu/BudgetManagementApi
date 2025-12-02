using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/configurations")]
public class AdminPremiumCalculationConfigurationController(
    IPremiumCalculationConfigurationService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all premium calculation configurations
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] string? portfolioAlias = null, [FromQuery] string? fiscalYear = null)
    {
        return HandleResult(await service.GetAllConfigurationsAsync(portfolioAlias, fiscalYear));
    }

    /// <summary>
    /// Get premium calculation configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetConfigurationByIdAsync(id));
    }

    /// <summary>
    /// Get premium calculation configuration by portfolio and fiscal year
    /// </summary>
    [HttpGet("portfolio/{portfolioAlias}/fiscal-year/{fiscalYear}")]
    public async Task<IActionResult> GetByPortfolioAndFiscalYearAsync(
        string portfolioAlias, 
        string fiscalYear, 
        [FromQuery] DateTime? effectiveDate = null)
    {
        return HandleResult(await service.GetConfigurationAsync(portfolioAlias, fiscalYear, effectiveDate));
    }

    /// <summary>
    /// Create a new premium calculation configuration
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePremiumCalculationConfigurationDto dto)
    {
        return HandleResult(await service.CreateConfigurationAsync(dto));
    }

    /// <summary>
    /// Update premium calculation configuration
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdatePremiumCalculationConfigurationDto dto)
    {
        return HandleResult(await service.UpdateConfigurationAsync(id, dto));
    }

    /// <summary>
    /// Delete premium calculation configuration (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await service.DeleteConfigurationAsync(id));
    }

    /// <summary>
    /// Activate a premium calculation configuration
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateAsync(string id, [FromBody] string fiscalYear)
    {
        return HandleResult(await service.ActivateConfigurationAsync(id, fiscalYear));
    }

    /// <summary>
    /// Clone configuration for a new fiscal year
    /// </summary>
    [HttpPost("{id}/clone")]
    public async Task<IActionResult> CloneAsync(string id, [FromBody] string newFiscalYear)
    {
        return HandleResult(await service.CloneConfigurationAsync(id, newFiscalYear));
    }

    /// <summary>
    /// Validate configuration
    /// </summary>
    [HttpPost("{id}/validate")]
    public async Task<IActionResult> ValidateAsync(string id)
    {
        return HandleResult(await service.ValidateConfigurationAsync(id));
    }
}

