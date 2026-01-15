using System.Text;
using System.Text.Json;
using Business.AdminPortalApi.ExcelExport;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class AttendanceService(
    ApplicationDataContext db,
    IHttpClientFactory httpClientFactory,
    ISieveExtension sieveExtension,
    IUserProfileService userProfileService,
    UserManager<ApplicationUser> userManager,
    ILogger<AttendanceService> logger,
    IConfiguration configuration,
    IExcelExportService excelExportService) : IAttendanceService
{
    public async Task<Result<AttendanceEntry>> CheckInAsync(string userId, double lat, double lng, string? remarks = null, CancellationToken cancellationToken = default)
    {
        var entry = new AttendanceEntry
        {
            UserId = userId,
            Type = "CheckIn",
            Latitude = lat,
            Longitude = lng,
            Timestamp = DateTime.UtcNow,
            Remarks = remarks
        };

        db.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        // Sync to tenant API in background (fire and forget)
        _ = Task.Run(async () => await SyncAttendanceToTenantApiAsync(entry, userId, cancellationToken), cancellationToken);

        return Result<AttendanceEntry>.Success(entry);
    }

    public async Task<Result<AttendanceEntry>> CheckOutAsync(string userId, double lat, double lng, string? remarks = null, CancellationToken cancellationToken = default)
    {
        var entry = new AttendanceEntry
        {
            UserId = userId,
            Type = "CheckOut",
            Latitude = lat,
            Longitude = lng,
            Timestamp = DateTime.UtcNow,
            Remarks = remarks
        };

        db.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        // Sync to tenant API in background (fire and forget)
        _ = Task.Run(async () => await SyncAttendanceToTenantApiAsync(entry, userId, cancellationToken), cancellationToken);

        return Result<AttendanceEntry>.Success(entry);
    }

    public async Task<Result<List<AttendanceEntry>>> GetDailyAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        var entries = await db.Set<AttendanceEntry>()
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Timestamp >= startDate && a.Timestamp < endDate)
            .OrderBy(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        return Result<List<AttendanceEntry>>.Success(entries);
    }

    public async Task<Result<List<AttendanceEntry>>> GetMonthlyAsync(string userId, int year, int month, CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var entries = await db.Set<AttendanceEntry>()
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Timestamp >= startDate && a.Timestamp < endDate)
            .OrderBy(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        return Result<List<AttendanceEntry>>.Success(entries);
    }

    public async Task<Result<bool>> HasAttendanceTodayAsync(string userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var hasAttendance = await db.Set<AttendanceEntry>()
            .AsNoTracking()
            .AnyAsync(a => a.UserId == userId &&
                          a.Timestamp >= today &&
                          a.Timestamp < tomorrow &&
                          a.Type == "CheckIn", cancellationToken);

        return Result<bool>.Success(hasAttendance);
    }

    public async Task<Result<List<AttendanceResponseDto>>> GetAttendanceForAdminAsync(
        CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            // Only check role for query filtering logic
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            // Build query
            IQueryable<AttendanceEntry> query = db.Set<AttendanceEntry>()
                .AsNoTracking();
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            // For SuperAdmin, ignore tenant filter to see all
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
            var attendanceEntries = await result
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);

            // Get user information
            var userIds = attendanceEntries.Select(a => a.UserId).Distinct().ToList();
            var users = await db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            // Get FoDo information
            // Note: IsDeleted filter is now applied globally


            // Get tenant names
            var tenantIds = attendanceEntries.Where(a => !string.IsNullOrEmpty(a.TenantId))
                .Select(a => a.TenantId)
                .Distinct()
                .ToList();
            var tenants = await db.Set<Data.Entities.Tenant.Tenant>()
                .Where(t => tenantIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

            // Map to DTOs
            var dtos = attendanceEntries.Select(entry =>
            {
                var userInfo = users.ContainsKey(entry.UserId) ? users[entry.UserId] : null;
                var tenantName = !string.IsNullOrEmpty(entry.TenantId) && tenants.ContainsKey(entry.TenantId)
                    ? tenants[entry.TenantId]
                    : null;

                return new AttendanceResponseDto(
                    entry.Id,
                    entry.UserId,
                    userInfo?.UserName,
                     userInfo?.UserName,
                    entry.Type,
                    entry.Latitude,
                    entry.Longitude,
                    entry.Timestamp,
                    entry.Remarks,
                    entry.TenantId,
                    tenantName,
                    entry.CreatedOn
                );
            }).ToList();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<AttendanceResponseDto>>.Success(dtos, pagination);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving attendance for admin: {Message}", ex.Message);
            return Result<List<AttendanceResponseDto>>.Failed($"An error occurred while retrieving attendance: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            requestModel.PageSize = -1;
            var result = await GetAttendanceForAdminAsync(requestModel, cancellationToken);

            if (!result.IsSuccess || result.Data == null)
                return Result<byte[]>.Failed(result.Error ?? "Failed to retrieve attendance data.");

            var columnMappings = new Dictionary<string, string>
            {
                { "Id", "ID" },
                { "UserId", "User ID" },
                { "UserName", "Username" },
                { "FullName", "Full Name" },
                { "Type", "Type" },
                { "Latitude", "Latitude" },
                { "Longitude", "Longitude" },
                { "Timestamp", "Timestamp" },
                { "Remarks", "Remarks" },
                { "TenantId", "Tenant ID" },
                { "TenantName", "Tenant Name" },
                { "CreatedOn", "Created On" }
            };

            var excelData = await excelExportService.ExportToExcelAsync(result.Data, "Attendance", columnMappings);
            return Result<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting attendance to Excel: {Message}", ex.Message);
            return Result<byte[]>.Failed($"An error occurred while exporting: {ex.Message}");
        }
    }

    private async Task SyncAttendanceToTenantApiAsync(AttendanceEntry entry, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(entry.TenantId))
                return;

            // Get tenant attendance API URL from configuration
            // Format: "TenantAttendanceApi:{tenantId}" or use a default pattern
            var apiUrlKey = $"TenantAttendanceApi:{entry.TenantId}";
            var apiUrl = configuration[apiUrlKey];

            if (string.IsNullOrEmpty(apiUrl))
            {
                // Try to get from tenant entity if it has AttendanceApiUrl property
                // For now, we'll use a default pattern or skip if not configured
                logger.LogDebug("Attendance API URL not configured for tenant {TenantId}", entry.TenantId);
                return;
            }

            // Get user and FoDo information
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            //var fodo = await db.Fodos.FirstOrDefaultAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);

            var syncDto = new TenantAttendanceSyncDto(
                entry.UserId,
                user?.UserName,
                 user?.UserName,
                entry.Type,
                entry.Latitude,
                entry.Longitude,
                entry.Timestamp,
                entry.Remarks
            );

            using var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var json = JsonSerializer.Serialize(syncDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Successfully synced attendance {AttendanceId} to tenant API", entry.Id);
            }
            else
            {
                logger.LogWarning("Failed to sync attendance {AttendanceId} to tenant API. Status: {Status}",
                    entry.Id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing attendance {AttendanceId} to tenant API: {Message}", entry.Id, ex.Message);
            // Don't throw - this is a background operation
        }
    }
}
