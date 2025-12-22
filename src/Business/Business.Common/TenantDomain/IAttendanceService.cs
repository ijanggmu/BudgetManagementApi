using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IAttendanceService
{
    Task<Result<AttendanceEntry>> CheckInAsync(string userId, double lat, double lng, string? remarks = null);
    Task<Result<AttendanceEntry>> CheckOutAsync(string userId, double lat, double lng, string? remarks = null);
    Task<Result<List<AttendanceEntry>>> GetDailyAsync(string userId, DateTime date);
    Task<Result<List<AttendanceEntry>>> GetMonthlyAsync(string userId, int year, int month);
    Task<Result<bool>> HasAttendanceTodayAsync(string userId);
    
    // Admin methods
    Task<Result<List<AttendanceResponseDto>>> GetAttendanceForAdminAsync(
        CommonPaginationRequestModel requestModel,
        string? userId = null,
        string? type = null,
        DateTime? from = null,
        DateTime? to = null,
        string? tenantId = null);
    
    Task<Result<byte[]>> ExportToExcelAsync(
        string? userId = null,
        string? type = null,
        DateTime? from = null,
        DateTime? to = null,
        string? tenantId = null);
}
