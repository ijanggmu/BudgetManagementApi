using Data.Context;
using Data.Infrastructure;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.Notification;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;
using NotificationEntity = Data.Entities.Tenant.Notification;

namespace Business.AdminPortalApi.Notification;

public class NotificationService : INotificationService
{
    private readonly ApplicationDataContext _db;
    private readonly INotificationSender _notificationSender;
    private readonly ITenantContext _tenantContext;
    private readonly IUserProfileService _userProfileService;
    private readonly ITenantResolutionService _tenantResolutionService;
    private readonly ISieveExtension _sieveExtension;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ApplicationDataContext db,
        INotificationSender notificationSender,
        ITenantContext tenantContext,
        IUserProfileService userProfileService,
        ITenantResolutionService tenantResolutionService,
        ISieveExtension sieveExtension,
        ILogger<NotificationService> logger)
    {
        _db = db;
        _notificationSender = notificationSender;
        _tenantContext = tenantContext;
        _userProfileService = userProfileService;
        _tenantResolutionService = tenantResolutionService;
        _sieveExtension = sieveExtension;
        _logger = logger;
    }

    public async Task<Result<MessageResponseModel>> SendNotificationAsync(
        SendNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<MessageResponseModel>.Failed("User not authenticated.");

        // Validate admin role - SuperAdmin or TenantAdmin (SystemRoles.Admin)
        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isTenantAdmin = userRoles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isTenantAdmin)
            return Result<MessageResponseModel>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId) && !isSuperAdmin)
            return Result<MessageResponseModel>.Failed("Tenant ID not found.");

        List<string> targetUserIds;
        string userTypeDescription;

        if (isSuperAdmin)
        {
            // SuperAdmin can send to ALL users (not just FoDo)
            // If tenant context is set, send to all users in that tenant
            // If no tenant context, send to all users across all tenants
            var usersQuery = _db.Users
                .Where(u => !u.IsDeleted && !u.IsDisabled);

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                // SuperAdmin without tenant context - send to all users across all tenants
                usersQuery = usersQuery.IgnoreQueryFilters();
                userTypeDescription = "all users across all tenants";
            }
            else
            {
                // SuperAdmin with tenant context - send to all users in that specific tenant
                usersQuery = usersQuery.Where(u => u.TenantId == tenantId);
                userTypeDescription = $"all users in tenant {tenantId}";
            }

            targetUserIds = await usersQuery
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // TenantAdmin (SystemRoles.Admin) can ONLY send to FoDo/MarketingExecutive role type users in their tenant
            // Query users who have roles with RoleType = FoDo or MarketingExecutive
            var fodoRoleTypes = new[] { SystemRoles.FoDo, SystemRoles.MarketingExecutive };

            targetUserIds = await _db.UserRoles
                .Join(_db.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, r.RoleType, r.TenantId, ur.IsDeleted })
                .Where(x => !x.IsDeleted &&
                           x.TenantId == tenantId &&
                           fodoRoleTypes.Contains(x.RoleType))
                .Select(x => x.UserId)
                .Distinct()
                .Join(_db.Users,
                    userId => userId,
                    u => u.Id,
                    (userId, u) => new { u.Id, u.IsDeleted, u.IsDisabled })
                .Where(x => !x.IsDeleted && !x.IsDisabled)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            userTypeDescription = $"FoDo/MarketingExecutive users in tenant {tenantId}";
        }

        if (!targetUserIds.Any())
        {
            var errorMessage = isSuperAdmin
                ? $"No active users found{(string.IsNullOrWhiteSpace(tenantId) ? " across all tenants" : $" in tenant {tenantId}")}."
                : "No active FoDo/Marketing Executive users found in the tenant.";
            return Result<MessageResponseModel>.Failed(errorMessage);
        }

        var sentCount = 0;
        var errors = new List<string>();

        foreach (var targetUserId in targetUserIds)
        {
            try
            {
                // Get user's tenant ID for proper notification isolation
                var userTenantId = await _db.Users
                    .Where(u => u.Id == targetUserId)
                    .Select(u => u.TenantId)
                    .FirstOrDefaultAsync(cancellationToken);

                // Create notification entity
                var notification = new NotificationEntity
                {
                    UserId = targetUserId,
                    Title = dto.Title,
                    Body = dto.Body,
                    Payload = dto.Payload,
                    Channel = dto.Channel,
                    SentAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    TenantId = userTenantId ?? string.Empty
                };

                await _db.Notifications.AddAsync(notification, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                // Send via SignalR if user is connected
                var notificationDto = new NotificationDto(
                    notification.Id,
                    notification.Title,
                    notification.Body,
                    notification.Payload,
                    notification.Channel,
                    notification.SentAt,
                    notification.ReadAt,
                    notification.IsRead
                );

                await _notificationSender.SendNotificationAsync(targetUserId, notificationDto, cancellationToken);
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", targetUserId);
                errors.Add($"Failed to send to user {targetUserId}: {ex.Message}");
            }
        }

        if (errors.Any() && sentCount == 0)
        {
            return Result<MessageResponseModel>.Failed($"Failed to send notifications: {string.Join("; ", errors)}");
        }

        var message = sentCount == targetUserIds.Count
            ? $"Notification sent successfully to {sentCount} user(s) ({userTypeDescription})."
            : $"Notification sent to {sentCount} of {targetUserIds.Count} user(s) ({userTypeDescription}). Some errors occurred: {string.Join("; ", errors)}";

        _logger.LogInformation("Notification sent by {UserId} ({Role}) to {Count} users ({UserType}) in tenant {TenantId}",
            userId, isSuperAdmin ? "SuperAdmin" : "TenantAdmin", sentCount, userTypeDescription, tenantId);

        return Result<MessageResponseModel>.Success(new MessageResponseModel(message));
    }

    public async Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(
        CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<List<NotificationDto>>.Failed("User not authenticated.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            return Result<List<NotificationDto>>.Failed("Tenant ID not found.");

        var query = _db.Notifications
            .Where(n => n.UserId == userId && n.TenantId == tenantId)
            .OrderByDescending(n => n.SentAt)
            .AsNoTracking();

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var notifications = await result.Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Body,
                n.Payload,
                n.Channel,
                n.SentAt,
                n.ReadAt,
                n.IsRead
            )).ToListAsync(cancellationToken);

            var pagination = new Pagination
            {
                TotalPages = totalPage,
                CurrentPage = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
                TotalItems = totalCount,
            };

            return Result<List<NotificationDto>>.Success(notifications, pagination);
        }

        var allNotifications = await query.Select(n => new NotificationDto(
            n.Id,
            n.Title,
            n.Body,
            n.Payload,
            n.Channel,
            n.SentAt,
            n.ReadAt,
            n.IsRead
        )).ToListAsync(cancellationToken);

        return Result<List<NotificationDto>>.Success(allNotifications);
    }

    public async Task<Result<NotificationDto>> MarkAsReadAsync(
        string notificationId,
        CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<NotificationDto>.Failed("User not authenticated.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            return Result<NotificationDto>.Failed("Tenant ID not found.");

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId &&
                                     n.UserId == userId &&
                                     n.TenantId == tenantId,
            cancellationToken);

        if (notification == null)
            return Result<NotificationDto>.Failed("Notification not found.");

        if (notification.IsRead)
            return Result<NotificationDto>.Failed("Notification is already marked as read.");

        notification.ReadAt = DateTime.UtcNow;
        notification.LastModifiedBy = userId;
        notification.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        var notificationDto = new NotificationDto(
            notification.Id,
            notification.Title,
            notification.Body,
            notification.Payload,
            notification.Channel,
            notification.SentAt,
            notification.ReadAt,
            notification.IsRead
        );

        return Result<NotificationDto>.Success(notificationDto);
    }

    public async Task<Result<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<int>.Failed("User not authenticated.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            return Result<int>.Failed("Tenant ID not found.");

        var count = await _db.Notifications
            .CountAsync(n => n.UserId == userId &&
                           n.TenantId == tenantId &&
                           n.ReadAt == null,
            cancellationToken);

        return Result<int>.Success(count);
    }
}

