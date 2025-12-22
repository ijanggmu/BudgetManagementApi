using System;
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
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel? requestModel)
    {
        return HandleResult(await service.ListAsync(requestModel));
    }

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetByIdAsync(id));
    }

    [HttpPost("Create")]
    [Permission(MenuPermissionConstant.TenantsCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantDto dto)
    {
        return HandleResult(await service.CreateAsync(dto));
    }

    [HttpPatch("{id}")]
    [Permission(MenuPermissionConstant.TenantsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTenantDto dto)
    {
        return HandleResult(await service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.TenantsDelete)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await service.DeleteAsync(id));
    }

    /// <summary>
    /// Get all active tenants for dropdown selection
    /// </summary>
    /// <returns>List of active tenants (Id, Name, Slug)</returns>
    [HttpGet("dropdown")]
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> GetTenantsForDropdownAsync()
    {
        return HandleResult(await service.GetTenantsForDropdownAsync());
    }

    /// <summary>
    /// Export all tenants to Excel
    /// </summary>
    /// <returns>Excel file</returns>
    [HttpGet("export")]
    [Permission(MenuPermissionConstant.TenantsExport)]
    public async Task<IActionResult> ExportToExcelAsync()
    {
        var result = await service.ExportToExcelAsync();
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Tenants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
