using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Common.Attendance;

[Route("api/v1/attendance")]
public class AttendanceController(IAttendanceService service, IUserProfileService userProfileService) : BaseCommonApiController
{
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] AttendanceDto dto)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await service.CheckInAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks));
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] AttendanceDto dto)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await service.CheckOutAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks));
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily([FromQuery] DateTime? date)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var targetDate = date ?? DateTime.UtcNow.Date;
        return HandleResult(await service.GetDailyAsync(userId, targetDate));
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int? year, [FromQuery] int? month)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        return HandleResult(await service.GetMonthlyAsync(userId, targetYear, targetMonth));
    }
}
