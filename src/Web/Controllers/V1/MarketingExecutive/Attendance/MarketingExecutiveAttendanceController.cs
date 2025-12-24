using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Attendance;
public class MarketingExecutiveAttendanceController(
    IAttendanceService attendanceService,
    IUserProfileService userProfileService) : BaseMarketingExecutiveApiController
{
    /// <summary>
    /// Check-in for Marketing Executive
    /// </summary>
    /// <param name="dto">Attendance check-in data with location</param>
    /// <returns>Attendance entry</returns>
    [HttpPost("check-in")]
    [Permission(MenuPermissionConstant.CommonAttendanceCreate)]
    public async Task<IActionResult> CheckIn([FromBody] AttendanceDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.CheckInAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks, cancellationToken));
    }

    /// <summary>
    /// Check-out for Marketing Executive
    /// </summary>
    /// <param name="dto">Attendance check-out data with location</param>
    /// <returns>Attendance entry</returns>
    [HttpPost("check-out")]
    [Permission(MenuPermissionConstant.CommonAttendanceCreate)]
    public async Task<IActionResult> CheckOut([FromBody] AttendanceDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.CheckOutAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks, cancellationToken));
    }

    /// <summary>
    /// Get daily attendance records for Marketing Executive
    /// </summary>
    /// <param name="date">Date to get attendance for (defaults to today)</param>
    /// <returns>List of attendance entries for the day</returns>
    [HttpGet("daily")]
    [Permission(MenuPermissionConstant.CommonAttendanceView)]
    public async Task<IActionResult> GetDaily([FromQuery] DateTime? date, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var targetDate = date ?? DateTime.UtcNow.Date;
        return HandleResult(await attendanceService.GetDailyAsync(userId, targetDate, cancellationToken));
    }

    /// <summary>
    /// Get monthly attendance records for Marketing Executive
    /// </summary>
    /// <param name="year">Year (defaults to current year)</param>
    /// <param name="month">Month (defaults to current month)</param>
    /// <returns>List of attendance entries for the month</returns>
    [HttpGet("monthly")]
    [Permission(MenuPermissionConstant.CommonAttendanceView)]
    public async Task<IActionResult> GetMonthly([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        return HandleResult(await attendanceService.GetMonthlyAsync(userId, targetYear, targetMonth, cancellationToken));
    }

    /// <summary>
    /// Check if Marketing Executive has performed attendance today
    /// </summary>
    /// <returns>True if check-in was performed today, false otherwise</returns>
    [HttpGet("today/status")]
    [Permission(MenuPermissionConstant.CommonAttendanceView)]
    public async Task<IActionResult> GetTodayStatus(CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.HasAttendanceTodayAsync(userId, cancellationToken));
    }
}

