using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.PremiumCalculation.Configuration;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.Admin.PremiumCalculation;

namespace BeemaEdgeApi.Controllers.V1.Admin.PremiumCalculation;

[Route("api/v1/admin/premium-calculation/rules")]
public class AdminPremiumCalculationRuleController(
    IPremiumCalculationRuleService service) : BaseAdminApiController
{
    /// <summary>
    /// Get all rules for a configuration
    /// </summary>
    [HttpGet("configuration/{configurationId}")]
    public async Task<IActionResult> GetByConfigurationAsync(string configurationId)
    {
        return HandleResult(await service.GetRulesByConfigurationIdAsync(configurationId));
    }

    /// <summary>
    /// Get rule by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetRuleByIdAsync(id));
    }

    /// <summary>
    /// Create a new rule
    /// </summary>
    [HttpPost("configuration/{configurationId}")]
    public async Task<IActionResult> CreateAsync(string configurationId, [FromBody] CreatePremiumCalculationRuleDto dto)
    {
        return HandleResult(await service.CreateRuleAsync(configurationId, dto));
    }

    /// <summary>
    /// Update a rule
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] CreatePremiumCalculationRuleDto dto)
    {
        return HandleResult(await service.UpdateRuleAsync(id, dto));
    }

    /// <summary>
    /// Delete a rule
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await service.DeleteRuleAsync(id));
    }
}

