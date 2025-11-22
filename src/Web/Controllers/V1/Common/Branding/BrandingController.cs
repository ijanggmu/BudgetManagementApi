using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Common.TenantDomain;

namespace BeemaEdgeApi.Controllers.V1.Common.Branding;

public class BrandingController : BaseCommonApiController
{
    private readonly IBrandingService _branding;

    public BrandingController(IBrandingService branding)
    {
        _branding = branding;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var branding = await _branding.GetAsync();
        if (branding is null) return NotFound();
        return Ok(branding);
    }
}


