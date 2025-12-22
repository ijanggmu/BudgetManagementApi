using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;

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
    public async Task<IActionResult> CheckIn([FromBody] AttendanceDto dto)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.CheckInAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks));
    }

    /// <summary>
    /// Check-out for Marketing Executive
    /// </summary>
    /// <param name="dto">Attendance check-out data with location</param>
    /// <returns>Attendance entry</returns>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] AttendanceDto dto)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.CheckOutAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks));
    }

    /// <summary>
    /// Get daily attendance records for Marketing Executive
    /// </summary>
    /// <param name="date">Date to get attendance for (defaults to today)</param>
    /// <returns>List of attendance entries for the day</returns>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily([FromQuery] DateTime? date)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var targetDate = date ?? DateTime.UtcNow.Date;
        return HandleResult(await attendanceService.GetDailyAsync(userId, targetDate));
    }

    /// <summary>
    /// Get monthly attendance records for Marketing Executive
    /// </summary>
    /// <param name="year">Year (defaults to current year)</param>
    /// <param name="month">Month (defaults to current month)</param>
    /// <returns>List of attendance entries for the month</returns>
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int? year, [FromQuery] int? month)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        return HandleResult(await attendanceService.GetMonthlyAsync(userId, targetYear, targetMonth));
    }

    /// <summary>
    /// Check if Marketing Executive has performed attendance today
    /// </summary>
    /// <returns>True if check-in was performed today, false otherwise</returns>
    [HttpGet("today/status")]
    public async Task<IActionResult> GetTodayStatus()
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return HandleResult(await attendanceService.HasAttendanceTodayAsync(userId));
    }
}

