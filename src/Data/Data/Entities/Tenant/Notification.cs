using System;

namespace Data.Entities.Tenant;

public class Notification : TenantEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Channel { get; set; } = "Push"; // Email, SMS, Push
    public string? Payload { get; set; } // JSON payload for deep linking
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead => ReadAt.HasValue;
}

