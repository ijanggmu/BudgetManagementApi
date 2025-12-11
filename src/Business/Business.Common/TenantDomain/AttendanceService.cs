using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Business.AdminPortalApi.ExcelExport;
using Data.Context;
using Data.Entities.FodoEntity;
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

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDataContext _db;

    public AttendanceService(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task<Result<AttendanceEntry>> CheckInAsync(string userId, double lat, double lng, string? remarks = null)
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

        _db.Add(entry);
        await _db.SaveChangesAsync();
        return Result<AttendanceEntry>.Success(entry);
    }

    public async Task<Result<AttendanceEntry>> CheckOutAsync(string userId, double lat, double lng, string? remarks = null)
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

        _db.Add(entry);
        await _db.SaveChangesAsync();
        return Result<AttendanceEntry>.Success(entry);
    }

    public async Task<Result<List<AttendanceEntry>>> GetDailyAsync(string userId, DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        var entries = await _db.Set<AttendanceEntry>()
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Timestamp >= startDate && a.Timestamp < endDate)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();

        return Result<List<AttendanceEntry>>.Success(entries);
    }

    public async Task<Result<List<AttendanceEntry>>> GetMonthlyAsync(string userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var entries = await _db.Set<AttendanceEntry>()
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Timestamp >= startDate && a.Timestamp < endDate)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();

        return Result<List<AttendanceEntry>>.Success(entries);
    }
}
