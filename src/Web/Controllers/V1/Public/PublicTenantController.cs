using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Public;

/// <summary>
/// Anonymous endpoints for public tenant discovery (e.g. tenant-scoped login URL validation).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/public/tenant")]
public class PublicTenantController(ITenantResolutionService tenantResolutionService) : ControllerBase
{
    /// <summary>
    /// Returns 200 if an active tenant exists for the slug; 404 otherwise.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var tenant = await tenantResolutionService.ResolveTenantBySlugAsync(slug);
        if (tenant == null)
            return NotFound();

        return Ok(new { slug = tenant.Slug });
    }
}
