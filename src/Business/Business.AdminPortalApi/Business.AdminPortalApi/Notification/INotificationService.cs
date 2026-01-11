using Microsoft.AspNetCore.SignalR;
using Models.Common;
using Models.WebApi.Notification;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Notification;

/// <summary>
/// Service for managing notifications with SignalR real-time delivery
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send notification to all FoDo/Marketing Executives in the current tenant
    /// </summary>
    Task<Result<MessageResponseModel>> SendNotificationAsync(SendNotificationDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get notifications for the current user with pagination
    /// </summary>
    Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    Task<Result<NotificationDto>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get unread notification count for the current user
    /// </summary>
    Task<Result<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default);

}

