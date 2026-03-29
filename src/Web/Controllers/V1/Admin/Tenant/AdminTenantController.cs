using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Tenants;

public class AdminTenantController(ITenantAdminService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.ListAsync(requestModel, cancellationToken));
    }

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("Create")]
    [Permission(MenuPermissionConstant.TenantsCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateAsync(dto, cancellationToken));
    }

    [HttpPatch("{id}")]
    [Permission(MenuPermissionConstant.TenantsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTenantDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateAsync(id, dto, cancellationToken));
    }

    /// <summary>Permanently deletes the tenant and all related data. SuperAdmin only; requires account password confirmation.</summary>
    [HttpPost("{id}/delete")]
    [SuperAdminOnly]
    public async Task<IActionResult> DeleteAsync(string id, [FromBody] DeleteTenantDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Get all active tenants for dropdown selection
    /// </summary>
    /// <returns>List of active tenants (Id, Name, Slug)</returns>
    [HttpGet("dropdown")]
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> GetTenantsForDropdownAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetTenantsForDropdownAsync(cancellationToken));
    }

    /// <summary>
    /// Export all tenants to Excel
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file</returns>
    [HttpPost("export")]
    [Permission(MenuPermissionConstant.TenantsExport)]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromBody] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var result = await service.ExportToExcelAsync(requestModel, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Tenants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
