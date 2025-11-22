using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Admin.Tenant;

[AllowAnonymous]
public class TenantAuthController(ITenantAuthService tenantAuthService) : BaseTenantAdminApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] TenantLoginRequestDto request)
    {
        return HandleResult(await tenantAuthService.LoginAsync(request));
    }
}

