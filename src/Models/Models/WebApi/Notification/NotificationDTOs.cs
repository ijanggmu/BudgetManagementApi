namespace Models.WebApi.Notification;

/// <summary>
/// DTO for sending a notification from admin to FoDo/Marketing Executives
/// </summary>
public record SendNotificationDto(
    string Title,
    string Body,
    string? Payload = null, // JSON payload for rich notifications (deep linking, actions, etc.)
    string Channel = "Push" // Email, SMS, Push
);

/// <summary>
/// DTO representing a notification
/// </summary>
public record NotificationDto(
    string Id,
    string Title,
    string Body,
    string? Payload,
    string Channel,
    DateTime SentAt,
    DateTime? ReadAt,
    bool IsRead
);

/// <summary>
/// Response DTO for unread notification count
/// </summary>
public record UnreadCountResponseDto(
    int Count
);

