using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IAttendanceService
{
    Task<Result<AttendanceEntry>> CheckInAsync(string userId, double lat, double lng, string? remarks = null);
    Task<Result<AttendanceEntry>> CheckOutAsync(string userId, double lat, double lng, string? remarks = null);
    Task<Result<List<AttendanceEntry>>> GetDailyAsync(string userId, DateTime date);
    Task<Result<List<AttendanceEntry>>> GetMonthlyAsync(string userId, int year, int month);
}
