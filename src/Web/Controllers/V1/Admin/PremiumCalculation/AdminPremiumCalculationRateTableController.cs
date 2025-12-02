using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/rate-tables")]
public class AdminPremiumCalculationRateTableController(
    IPremiumCalculationRateTableService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all rate tables for a configuration
    /// </summary>
    [HttpGet("configuration/{configurationId}")]
    public async Task<IActionResult> GetByConfigurationAsync(string configurationId)
    {
        return HandleResult(await service.GetRateTablesByConfigurationIdAsync(configurationId));
    }

    /// <summary>
    /// Get rate table by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetRateTableByIdAsync(id));
    }

    /// <summary>
    /// Create a new rate table
    /// </summary>
    [HttpPost("configuration/{configurationId}")]
    public async Task<IActionResult> CreateAsync(string configurationId, [FromBody] CreatePremiumCalculationRateTableDto dto)
    {
        return HandleResult(await service.CreateRateTableAsync(configurationId, dto));
    }

    /// <summary>
    /// Update a rate table
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreatePremiumCalculationRateTableDto dto)
    {
        return HandleResult(await service.UpdateRateTableAsync(id, dto));
    }

    /// <summary>
    /// Delete a rate table
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await service.DeleteRateTableAsync(id));
    }
}

