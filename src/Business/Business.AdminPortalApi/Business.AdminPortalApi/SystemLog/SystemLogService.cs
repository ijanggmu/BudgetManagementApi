using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Audit.UserActivites;
using Data.Infrastructure;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace AdminPortalApi.Controllers.V1.SystemLog;
public class AccessLogResponseModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string IpAddress { get; set; }
    public string At { get; set; }
    public string EndAt { get; set; }

    public string RequestHost { get; set; }
    public string UserAgent { get; set; }

    public string RequestPath { get; set; }
    public string RequestPathAlias { get; set; }
    /// <summary>User-friendly activity name for display in UI.</summary>
    public string ActivityDisplayName { get; set; }
    public string RequestMethod { get; set; }
    public string RequestQueryString { get; set; }
    public string RequestBody { get; set; }

    public int ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public string CorrelationId { get; set; }
    public double ResponseTimeInMS { get; set; }
    public string Module { get; set; }
    public string RequestHeader { get; set; }
    public string TenantId { get; set; }
}
public class SystemLogService : ISystemLogService
{
    private readonly ISieveExtension _sieveExtension;
    private readonly AuditDataContext _auditContext;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantResolutionService _tenantResolutionService;
    private readonly IUserProfileService _userProfileService;

    public SystemLogService(
        ISieveExtension sieveExtension,
        AuditDataContext auditContext,
        ITenantContext tenantContext,
        ITenantResolutionService tenantResolutionService,
        IUserProfileService userProfileService)
    {
        _sieveExtension = sieveExtension;
        _auditContext = auditContext;
        _tenantContext = tenantContext;
        _tenantResolutionService = tenantResolutionService;
        _userProfileService = userProfileService;
    }
    
    public async Task<Result<List<AccessLogResponseModel>>> GetAllSystemAccessLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
    {
        // Only showable logs (visible to users in activity lists)
        var query = _auditContext.UserActivities
            .Where(x => x.VisibleToUserExceptAdmin)
            .AsQueryable();

        // Scope: SuperAdmin = all tenants; Admin = their tenant only (caller is Admin/SuperAdmin by permission)
        var userId = _userProfileService.GetUserId();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

            if (!isSuperAdmin && !string.IsNullOrWhiteSpace(_tenantContext?.TenantId))
                query = query.Where(x => x.TenantId == _tenantContext.TenantId);
        }

        var defaultSort = $"-{nameof(UserActivity.At)}";
        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, searchModel, null, defaultSort);

        var response = await result.Select(x => new AccessLogResponseModel
        {
            Id = x.Id,
            UserName = x.UserName,
            IpAddress = x.IpAddress,
            At = x.At.ToString(),
            Module = x.Module,
            RequestBody = x.RequestBody,
            RequestHost = x.RequestHost,
            RequestMethod = x.RequestMethod,
            RequestPathAlias = x.RequestPathAlias,
            ActivityDisplayName = ActivityDisplayNameHelper.GetDisplayName(x.RequestPath, x.RequestPathAlias, x.Module),
            RequestQueryString = x.RequestQueryString,
            ResponseBody = x.ResponseBody,
            ResponseStatusCode = x.ResponseStatusCode,
            UserAgent = x.UserAgent,
            RequestPath = x.RequestPath,
            CorrelationId = x.CorrelationId,
            ResponseTimeInMS = x.ResponseTime,
            EndAt = x.EndAt.ToString(),
            RequestHeader = x.RequestHeader,
            TenantId = x.TenantId,
        }).ToListAsync(cancellationToken);

        return Result<List<AccessLogResponseModel>>.Success(response, new Pagination
        {
            TotalItems = totalCount,
            CurrentPage = searchModel.PageNumber,
            PageSize = searchModel.PageSize,
            TotalPages = totalPage,
        });
    }

    /// <summary>
    /// Returns recent showable activities for the current user's scope (own / tenant / all).
    /// Used by dashboard. Individual = own, Admin = all tenant users, SuperAdmin = all.
    /// </summary>
    public async Task<Result<List<AccessLogResponseModel>>> GetRecentActivityAsync(int limit, CancellationToken cancellationToken = default)
    {
        var query = await ApplyActivityScopeAsync(_auditContext.UserActivities.Where(x => x.VisibleToUserExceptAdmin), cancellationToken);
        var list = await query
            .OrderByDescending(x => x.At)
            .Take(limit)
            .Select(x => new AccessLogResponseModel
            {
                Id = x.Id,
                UserName = x.UserName,
                IpAddress = x.IpAddress,
                At = x.At.ToString(),
                RequestPath = x.RequestPath,
                RequestPathAlias = x.RequestPathAlias,
                ActivityDisplayName = ActivityDisplayNameHelper.GetDisplayName(x.RequestPath, x.RequestPathAlias, x.Module),
                Module = x.Module,
                RequestHost = x.RequestHost,
                RequestMethod = x.RequestMethod,
                ResponseStatusCode = x.ResponseStatusCode,
                EndAt = x.EndAt.ToString(),
                TenantId = x.TenantId,
            })
            .ToListAsync(cancellationToken);
        return Result<List<AccessLogResponseModel>>.Success(list);
    }

    /// <summary>
    /// Paginated activity log with same scope as recent activity. For Activity Log page (individual = own, admin = tenant, superadmin = all).
    /// </summary>
    public async Task<Result<List<AccessLogResponseModel>>> GetActivityLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
    {
        var baseQuery = _auditContext.UserActivities.Where(x => x.VisibleToUserExceptAdmin);
        var query = await ApplyActivityScopeAsync(baseQuery, cancellationToken);
        var defaultSort = $"-{nameof(UserActivity.At)}";
        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, searchModel, null, defaultSort);
        var response = await result.Select(x => new AccessLogResponseModel
        {
            Id = x.Id,
            UserName = x.UserName,
            IpAddress = x.IpAddress,
            At = x.At.ToString(),
            EndAt = x.EndAt.ToString(),
            RequestPath = x.RequestPath,
            RequestPathAlias = x.RequestPathAlias,
            ActivityDisplayName = ActivityDisplayNameHelper.GetDisplayName(x.RequestPath, x.RequestPathAlias, x.Module),
            Module = x.Module,
            RequestHost = x.RequestHost,
            RequestMethod = x.RequestMethod,
            ResponseStatusCode = x.ResponseStatusCode,
            TenantId = x.TenantId,
        }).ToListAsync(cancellationToken);
        return Result<List<AccessLogResponseModel>>.Success(response, new Pagination
        {
            TotalItems = totalCount,
            CurrentPage = searchModel.PageNumber,
            PageSize = searchModel.PageSize,
            TotalPages = totalPage,
        });
    }

    /// <summary>
    /// Apply role-based scope: Individual = own user, Admin = tenant, SuperAdmin = all.
    /// </summary>
    private async Task<IQueryable<UserActivity>> ApplyActivityScopeAsync(IQueryable<UserActivity> query, CancellationToken cancellationToken)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return query.Where(_ => false);

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        if (userRoles.Contains(SystemRoles.SuperAdmin))
            return query;
        if (userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin"))
        {
            if (!string.IsNullOrWhiteSpace(_tenantContext?.TenantId))
                return query.Where(x => x.TenantId == _tenantContext.TenantId);
            return query;
        }
        return query.Where(x => x.UserId == userId);
    }
}
