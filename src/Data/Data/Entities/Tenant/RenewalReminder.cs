using System;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class RenewalReminder : TenantEntity
{
    public string PolicyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime ReminderSentAt { get; set; }
    public string Channel { get; set; } = string.Empty; // Email, SMS, Push
}
