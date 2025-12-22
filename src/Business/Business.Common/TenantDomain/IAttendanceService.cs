using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IAttendanceService
{
    Task<Result<AttendanceEntry>> CheckInAsync(string userId, double lat, double lng, string? remarks = null, CancellationToken cancellationToken = default);
    Task<Result<AttendanceEntry>> CheckOutAsync(string userId, double lat, double lng, string? remarks = null, CancellationToken cancellationToken = default);
    Task<Result<List<AttendanceEntry>>> GetDailyAsync(string userId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<List<AttendanceEntry>>> GetMonthlyAsync(string userId, int year, int month, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasAttendanceTodayAsync(string userId, CancellationToken cancellationToken = default);
    
    // Admin methods
    Task<Result<List<AttendanceResponseDto>>> GetAttendanceForAdminAsync(
        CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    
    Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}
