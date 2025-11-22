using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Admin.Branding;

public class AdminBrandingController : BaseAdminApiController
{
    private readonly IBrandingService _branding;

    public AdminBrandingController(IBrandingService branding)
    {
        _branding = branding;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var branding = await _branding.GetAsync();
        if (branding is null) return NotFound();
        return Ok(branding);
    }

    public record UpdateBrandingRequest(string? LogoUrl, string? PaletteJson, string? TypographyJson);

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateBrandingRequest request)
    {
        var branding = await _branding.UpdateAsync(request.LogoUrl, request.PaletteJson, request.TypographyJson);
        return Ok(branding);
    }
}


