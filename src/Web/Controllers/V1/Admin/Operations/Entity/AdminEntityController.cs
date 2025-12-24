using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Entity;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Operations.Entity;

public class AdminEntityController(IEntitySettingsService entitySettingsService) : BaseAdminApiController
{
    /// <summary>
    /// Get entity settings (Primary Color, Logo, Underwriter Digital Signature, Underwriter Name)
    /// </summary>
    /// <returns>Entity settings</returns>
    [HttpGet]
    [Permission(MenuPermissionConstant.EntitySettingsView)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        => HandleResult(await entitySettingsService.GetAsync(cancellationToken));

    /// <summary>
    /// Update entity settings (Underwriter Digital Signature and Underwriter Name only)
    /// Primary Color and Logo are readonly
    /// </summary>
    /// <param name="dto">Entity settings update data</param>
    /// <returns>Updated entity settings</returns>
    [HttpPut]
    [Permission(MenuPermissionConstant.EntitySettingsUpdate)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await entitySettingsService.UpdateAsync(dto, cancellationToken));
}

