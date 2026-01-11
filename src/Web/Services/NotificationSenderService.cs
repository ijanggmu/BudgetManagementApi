using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Hubs;
using Business.AdminPortalApi.Notification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationDto = Models.WebApi.Notification.NotificationDto;

namespace BeemaEdgeApi.Services;

/// <summary>
/// Implementation of INotificationSender that sends notifications via SignalR
/// This is in the Web project to avoid circular dependency with Business.AdminPortalApi
/// </summary>
public class NotificationSenderService : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationSenderService> _logger;

    public NotificationSenderService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationSenderService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationAsync(
        string userId,
        NotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var groupName = $"user_{userId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification, cancellationToken);
            
            _logger.LogDebug("Notification sent via SignalR to user {UserId} in group {GroupName}", userId, groupName);
        }
        catch (Exception ex)
        {
            // Log but don't fail - notification is already persisted in DB
            _logger.LogWarning(ex, "Failed to send notification via SignalR to user {UserId}. Notification is persisted and will be available on next connection.", userId);
        }
    }
}

