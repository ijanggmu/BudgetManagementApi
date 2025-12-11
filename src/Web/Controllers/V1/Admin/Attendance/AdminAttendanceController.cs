using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Attendance;

public class AdminAttendanceController(IAttendanceService attendanceService) : BaseAdminApiController
{
    /// <summary>
    /// Get all attendance records for Admin (filtered by tenant) or SuperAdmin (all tenants)
    /// </summary>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <param name="userId">Optional: Filter by user ID</param>
    /// <param name="type">Optional: Filter by type (CheckIn, CheckOut)</param>
    /// <param name="from">Optional: Filter attendance from this date</param>
    /// <param name="to">Optional: Filter attendance until this date</param>
    /// <param name="tenantId">Optional: Filter by tenant ID (SuperAdmin only)</param>
    /// <returns>Paginated list of attendance records</returns>
    [HttpGet]
    [Permission(MenuPermissionConstant.OperationsView)]
    public async Task<IActionResult> GetAttendanceAsync(
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? userId = null,
        [FromQuery] string? type = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? tenantId = null)
    {
        return HandleResult(await attendanceService.GetAttendanceForAdminAsync(
            requestModel, userId, type, from, to, tenantId));
    }

    /// <summary>
    /// Export attendance records to Excel
    /// </summary>
    /// <param name="userId">Optional: Filter by user ID</param>
    /// <param name="type">Optional: Filter by type (CheckIn, CheckOut)</param>
    /// <param name="from">Optional: Filter attendance from this date</param>
    /// <param name="to">Optional: Filter attendance until this date</param>
    /// <param name="tenantId">Optional: Filter by tenant ID (SuperAdmin only)</param>
    /// <returns>Excel file</returns>
    [HttpGet("export")]
    [Permission(MenuPermissionConstant.OperationsView)] // Note: Export permission can be added if needed
    public async Task<IActionResult> ExportToExcelAsync(
        [FromQuery] string? userId = null,
        [FromQuery] string? type = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? tenantId = null)
    {
        var result = await attendanceService.ExportToExcelAsync(userId, type, from, to, tenantId);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Attendance_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

