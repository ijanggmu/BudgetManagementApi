using System;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class NotificationHistory : TenantEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // Email, SMS, Push
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
