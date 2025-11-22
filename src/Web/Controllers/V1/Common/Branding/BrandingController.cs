using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Common.TenantDomain;

namespace BeemaEdgeApi.Controllers.V1.Common.Branding;

public class BrandingController(IBrandingService brandingService) : BaseCommonApiController
{
    /// <summary>
    /// Get branding for current tenant (public endpoint)
    /// </summary>
    /// <returns>Branding information</returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync()
        => HandleResult(await brandingService.GetAsync());
}


