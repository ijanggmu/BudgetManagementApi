using Data.Context;
using Data.Entities.FodoEntity;
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

        // Validate admin role
        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<MessageResponseModel>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId) && !isSuperAdmin)
            return Result<MessageResponseModel>.Failed("Tenant ID not found.");

        // Get all FoDo/Marketing Executive users
        var fodoQuery = _db.Fodos
            .Include(f => f.User)
            .Where(f => !f.User.IsDeleted && f.IsActive);

        // For SuperAdmin without tenant context, send to all tenants (ignore query filters)
        // For SuperAdmin with tenant context, send to that specific tenant
        // For TenantAdmin, only their tenant's FoDo users
        if (isSuperAdmin && string.IsNullOrWhiteSpace(tenantId))
        {
            // SuperAdmin without tenant context - send to all tenants
            fodoQuery = fodoQuery.IgnoreQueryFilters();
        }
        else
        {
            // Filter by tenant (for both SuperAdmin with tenant context and TenantAdmin)
            fodoQuery = fodoQuery.Where(f => f.TenantId == tenantId);
        }

        var fodos = await fodoQuery.ToListAsync(cancellationToken);

        if (!fodos.Any())
            return Result<MessageResponseModel>.Failed("No active FoDo/Marketing Executive users found in the tenant.");

        var sentCount = 0;
        var errors = new List<string>();

        foreach (var fodo in fodos)
        {
            try
            {
                // Create notification entity
                var notification = new NotificationEntity
                {
                    UserId = fodo.UserId,
                    Title = dto.Title,
                    Body = dto.Body,
                    Payload = dto.Payload,
                    Channel = dto.Channel,
                    SentAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                // Set TenantId (use fodo's tenant ID to ensure proper isolation)
                notification.TenantId = fodo.TenantId;

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

                await _notificationSender.SendNotificationAsync(fodo.UserId, notificationDto, cancellationToken);
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to FoDo {FoDoId} (UserId: {UserId})", 
                    fodo.Id, fodo.UserId);
                errors.Add($"Failed to send to {fodo.FullName}: {ex.Message}");
            }
        }

        if (errors.Any() && sentCount == 0)
        {
            return Result<MessageResponseModel>.Failed($"Failed to send notifications: {string.Join("; ", errors)}");
        }

        var message = sentCount == fodos.Count
            ? $"Notification sent successfully to {sentCount} user(s)."
            : $"Notification sent to {sentCount} of {fodos.Count} user(s). Some errors occurred: {string.Join("; ", errors)}";

        _logger.LogInformation("Notification sent by {UserId} to {Count} FoDo users in tenant {TenantId}", 
            userId, sentCount, tenantId);

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

