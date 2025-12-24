using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Attendance;

[Route("api/v1/attendance")]
public class AttendanceController(IAttendanceService service, IUserProfileService userProfileService) : BaseCommonApiController
{
    [HttpPost("check-in")]
    [Permission(MenuPermissionConstant.CommonAttendanceCreate)]
    public async Task<IActionResult> CheckIn([FromBody] AttendanceDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await service.CheckInAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks, cancellationToken));
    }

    [HttpPost("check-out")]
    [Permission(MenuPermissionConstant.CommonAttendanceCreate)]
    public async Task<IActionResult> CheckOut([FromBody] AttendanceDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await service.CheckOutAsync(userId, dto.Latitude, dto.Longitude, dto.Remarks, cancellationToken));
    }

    [HttpGet("daily")]
    [Permission(MenuPermissionConstant.CommonAttendanceView)]
    public async Task<IActionResult> GetDaily([FromQuery] DateTime? date, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var targetDate = date ?? DateTime.UtcNow.Date;
        return HandleResult(await service.GetDailyAsync(userId, targetDate, cancellationToken));
    }

    [HttpGet("monthly")]
    [Permission(MenuPermissionConstant.CommonAttendanceView)]
    public async Task<IActionResult> GetMonthly([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        return HandleResult(await service.GetMonthlyAsync(userId, targetYear, targetMonth, cancellationToken));
    }
}
