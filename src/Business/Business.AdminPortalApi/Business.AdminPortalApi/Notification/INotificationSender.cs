using Models.WebApi.Notification;

namespace Business.AdminPortalApi.Notification;

/// <summary>
/// Interface for sending notifications via SignalR
/// Implemented in Web project to avoid circular dependency
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// Send notification to a specific user via SignalR
    /// </summary>
    Task SendNotificationAsync(string userId, NotificationDto notification, CancellationToken cancellationToken = default);
}

