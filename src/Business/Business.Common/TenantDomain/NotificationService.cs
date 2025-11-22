using System;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;

namespace Business.Common.TenantDomain;

public class NotificationService : INotificationService
{
    private readonly ApplicationDataContext _db;
    // In a real app, inject IEmailSender, ISmsSender, etc.

    public NotificationService(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task SendEmailAsync(string userId, string subject, string body)
    {
        // Simulate sending email
        await LogNotificationAsync(userId, "Email", userId, subject, body, true);
    }

    public async Task SendSmsAsync(string userId, string message)
    {
        // Simulate sending SMS
        await LogNotificationAsync(userId, "SMS", userId, "SMS", message, true);
    }

    public async Task SendPushAsync(string userId, string title, string body)
    {
        // Simulate sending Push
        await LogNotificationAsync(userId, "Push", userId, title, body, true);
    }

    private async Task LogNotificationAsync(string userId, string channel, string recipient, string subject, string body, bool success, string? error = null)
    {
        var history = new NotificationHistory
        {
            UserId = userId,
            Channel = channel,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            SentAt = DateTime.UtcNow,
            IsSuccess = success,
            ErrorMessage = error
        };

        _db.Add(history);
        await _db.SaveChangesAsync();
    }
}
