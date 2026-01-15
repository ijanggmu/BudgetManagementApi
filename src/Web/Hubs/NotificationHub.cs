using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedKernel.Constant;

namespace BeemaEdgeApi.Hubs;

/// <summary>
/// SignalR Hub for real-time notifications
/// Handles user connections, groups, and notification delivery
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Connection attempt without authenticated user. ConnectionId: {ConnectionId}", Context.ConnectionId);
            Context.Abort();
            return;
        }

        // Add user to their personal group for targeted notifications
        var groupName = $"user_{userId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation("User {UserId} connected. ConnectionId: {ConnectionId}, Group: {GroupName}",
            userId, Context.ConnectionId, groupName);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            var groupName = $"user_{userId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation("User {UserId} disconnected. ConnectionId: {ConnectionId}, Group: {GroupName}",
                userId, Context.ConnectionId, groupName);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "User disconnected with exception. ConnectionId: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Note: OnReconnectedAsync is not available in SignalR Hub base class
    // SignalR automatically handles reconnection and will call OnConnectedAsync again
    // The reconnection logic is handled in OnConnectedAsync

    /// <summary>
    /// Gets the user ID from the authenticated user's claims
    /// </summary>
    private string GetUserId()
    {
        return Context.User?.FindFirst(TokenKey.UserId)?.Value;
    }
}

