using Data.Context;
using Data.Infrastructure;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.Noticeboard;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;
using NoticeboardEntity = Data.Entities.Tenant.Noticeboard;

namespace Business.AdminPortalApi.Noticeboard;

public class NoticeboardService : INoticeboardService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly ITenantContext _tenantContext;
    private readonly IUserProfileService _userProfileService;
    private readonly ITenantResolutionService _tenantResolutionService;
    private readonly ILogger<NoticeboardService> _logger;

    public NoticeboardService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        ITenantContext tenantContext,
        IUserProfileService userProfileService,
        ITenantResolutionService tenantResolutionService,
        ILogger<NoticeboardService> logger)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _tenantContext = tenantContext;
        _userProfileService = userProfileService;
        _tenantResolutionService = tenantResolutionService;
        _logger = logger;
    }

    public async Task<Result<List<NoticeboardResponseDto>>> GetAllNoticeboardsAsync(
        CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<List<NoticeboardResponseDto>>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var tenantId = _tenantContext.TenantId;

        IQueryable<NoticeboardEntity> query = _db.Noticeboards.AsNoTracking();

        // For SuperAdmin without tenant context, show all notices
        // For SuperAdmin with tenant context or TenantAdmin, filter by tenant
        if (isSuperAdmin && string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.IgnoreQueryFilters();
        }
        else if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(n => n.TenantId == tenantId);
        }
        else
        {
            return Result<List<NoticeboardResponseDto>>.Failed("Tenant ID not found.");
        }

        // Apply ordering after filtering
        query = query
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.Priority)
            .ThenByDescending(n => n.CreatedOn);

        // Filter active notices only (unless admin wants to see all)
        // For now, we'll show all notices. Admin can filter by IsActive if needed

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var noticeboards = await result.Select(n => MapToResponse(n)).ToListAsync(cancellationToken);

            var pagination = new Pagination
            {
                TotalPages = totalPage,
                CurrentPage = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
                TotalItems = totalCount,
            };

            return Result<List<NoticeboardResponseDto>>.Success(noticeboards, pagination);
        }

        var allNoticeboards = await query.Select(n => MapToResponse(n)).ToListAsync(cancellationToken);
        return Result<List<NoticeboardResponseDto>>.Success(allNoticeboards);
    }

    public async Task<Result<NoticeboardResponseDto>> GetNoticeboardByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<NoticeboardResponseDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var tenantId = _tenantContext.TenantId;

        var query = _db.Noticeboards.AsNoTracking();

        if (isSuperAdmin && string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.IgnoreQueryFilters();
        }
        else if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(n => n.TenantId == tenantId);
        }
        else
        {
            return Result<NoticeboardResponseDto>.Failed("Tenant ID not found.");
        }

        var noticeboard = await query.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        if (noticeboard == null)
            return Result<NoticeboardResponseDto>.Failed("Noticeboard entry not found.");

        return Result<NoticeboardResponseDto>.Success(MapToResponse(noticeboard));
    }

    public async Task<Result<NoticeboardResponseDto>> CreateNoticeboardAsync(
        CreateNoticeboardDto dto,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<NoticeboardResponseDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<NoticeboardResponseDto>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId) && !isSuperAdmin)
            return Result<NoticeboardResponseDto>.Failed("Tenant ID is required.");

        // Validate dates
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
            return Result<NoticeboardResponseDto>.Failed("Start date cannot be after end date.");

        var noticeboard = new NoticeboardEntity
        {
            Title = dto.Title,
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            IsPinned = dto.IsPinned,
            Priority = dto.Priority,
            TenantId = tenantId ?? string.Empty, // SuperAdmin can set tenant context
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };

        await _db.Noticeboards.AddAsync(noticeboard, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Noticeboard entry created by {UserId} in tenant {TenantId}", userId, tenantId);

        return Result<NoticeboardResponseDto>.Success(MapToResponse(noticeboard));
    }

    public async Task<Result<NoticeboardResponseDto>> UpdateNoticeboardAsync(
        string id,
        UpdateNoticeboardDto dto,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<NoticeboardResponseDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<NoticeboardResponseDto>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;

        var query = _db.Noticeboards.AsQueryable();

        if (isSuperAdmin && string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.IgnoreQueryFilters();
        }
        else if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(n => n.TenantId == tenantId);
        }
        else
        {
            return Result<NoticeboardResponseDto>.Failed("Tenant ID not found.");
        }

        var noticeboard = await query.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        if (noticeboard == null)
            return Result<NoticeboardResponseDto>.Failed("Noticeboard entry not found.");

        // Validate dates
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
            return Result<NoticeboardResponseDto>.Failed("Start date cannot be after end date.");

        noticeboard.Title = dto.Title;
        noticeboard.Content = dto.Content;
        noticeboard.ImageUrl = dto.ImageUrl;
        noticeboard.StartDate = dto.StartDate;
        noticeboard.EndDate = dto.EndDate;
        noticeboard.IsActive = dto.IsActive;
        noticeboard.IsPinned = dto.IsPinned;
        noticeboard.Priority = dto.Priority;
        noticeboard.LastModifiedBy = userId;
        noticeboard.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Noticeboard entry {Id} updated by {UserId} in tenant {TenantId}", id, userId, tenantId);

        return Result<NoticeboardResponseDto>.Success(MapToResponse(noticeboard));
    }

    public async Task<Result<MessageResponseModel>> DeleteNoticeboardAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<MessageResponseModel>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<MessageResponseModel>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;

        var query = _db.Noticeboards.AsQueryable();

        if (isSuperAdmin && string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.IgnoreQueryFilters();
        }
        else if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(n => n.TenantId == tenantId);
        }
        else
        {
            return Result<MessageResponseModel>.Failed("Tenant ID not found.");
        }

        var noticeboard = await query.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        if (noticeboard == null)
            return Result<MessageResponseModel>.Failed("Noticeboard entry not found.");

        // Soft delete
        noticeboard.IsDeleted = true;
        noticeboard.LastModifiedBy = userId;
        noticeboard.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Noticeboard entry {Id} deleted by {UserId} in tenant {TenantId}", id, userId, tenantId);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Noticeboard entry deleted successfully."));
    }

    private static NoticeboardResponseDto MapToResponse(NoticeboardEntity noticeboard)
    {
        return new NoticeboardResponseDto
        {
            Id = noticeboard.Id,
            Title = noticeboard.Title,
            Content = noticeboard.Content,
            ImageUrl = noticeboard.ImageUrl,
            StartDate = noticeboard.StartDate,
            EndDate = noticeboard.EndDate,
            IsActive = noticeboard.IsActive,
            IsPinned = noticeboard.IsPinned,
            Priority = noticeboard.Priority,
            CreatedOn = noticeboard.CreatedOn,
            CreatedBy = noticeboard.CreatedBy,
            LastModifiedOn = noticeboard.LastModifiedOn,
            LastModifiedBy = noticeboard.LastModifiedBy
        };
    }
}
