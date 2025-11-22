using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Admin.Branding;

[Route("api/v1/admin/branding")]
public class AdminBrandingController(IBrandingService brandingService) : BaseAdminApiController
{
    /// <summary>
    /// Get branding for current tenant (Tenant Admin) or by tenantId (SuperAdmin)
    /// </summary>
    /// <param name="tenantId">Optional: Tenant ID (SuperAdmin only)</param>
    /// <returns>Branding information</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] string? tenantId = null)
    {
        if (!string.IsNullOrEmpty(tenantId))
            return HandleResult(await brandingService.GetByTenantIdAsync(tenantId));
        
        return HandleResult(await brandingService.GetAsync());
    }

    /// <summary>
    /// Update branding for current tenant
    /// </summary>
    /// <param name="dto">Branding update data</param>
    /// <returns>Updated branding information</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateBrandingDto dto)
        => HandleResult(await brandingService.UpdateAsync(dto));
}


