using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Attendance;

[AdminOrSuperAdmin] // All endpoints require Admin or SuperAdmin role
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
    [HttpPost]
    [Permission(MenuPermissionConstant.AttendanceView)]
    public async Task<IActionResult> GetAttendanceAsync(
        [FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return HandleResult(await attendanceService.GetAttendanceForAdminAsync(
            requestModel, cancellationToken));
    }

    /// <summary>
    /// Export attendance records to Excel
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file</returns>
    [HttpPost("export")]
    [Permission(MenuPermissionConstant.AttendanceExport)]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromBody] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var result = await attendanceService.ExportToExcelAsync(requestModel, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Attendance_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

