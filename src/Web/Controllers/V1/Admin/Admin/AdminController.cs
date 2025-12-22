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

namespace BeemaEdgeApi.Controllers.V1.Admin.Admin;


public class AdminController(IAdminService adminService) : BaseAdminApiController
{
    /// <summary>
    /// Get all admins (for tenant admin: their tenant's admins, for superadmin: all admins or filtered by tenantId)
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of admins</returns>
    [HttpGet]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null, CancellationToken cancellationToken = default)
        => HandleResult(await adminService.GetAdminsForAdminAsync(tenantId, cancellationToken));

    /// <summary>
    /// Get admin by ID
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <returns>Admin details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await adminService.GetAdminByIdAsync(id, cancellationToken));

    /// <summary>
    /// Create a new admin user
    /// </summary>
    /// <param name="dto">Admin creation data</param>
    /// <returns>Created admin details</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.AdminManagementCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAdminDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await adminService.CreateAsync(dto, cancellationToken));

    /// <summary>
    /// Update admin user
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <param name="dto">Admin update data</param>
    /// <returns>Updated admin details</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.AdminManagementUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateAdminDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await adminService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>
    /// Delete admin user (soft delete)
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.AdminManagementDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await adminService.DeleteAsync(id, cancellationToken));

    /// <summary>
    /// Export all admins to Excel
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file</returns>
    [HttpPost("export")]
    [Permission(MenuPermissionConstant.AdminManagementExport)]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromBody] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var result = await adminService.ExportToExcelAsync(requestModel, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Admins_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

