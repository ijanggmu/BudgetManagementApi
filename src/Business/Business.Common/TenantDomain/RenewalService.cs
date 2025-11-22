using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Business.Common.TenantDomain;

public class RenewalService : IRenewalService
{
    private readonly ApplicationDataContext _db;
    private readonly INotificationService _notificationService;

    public RenewalService(ApplicationDataContext db, INotificationService notificationService)
    {
        _db = db;
        _notificationService = notificationService;
    }

    public async Task CheckAndSendRemindersAsync()
    {
        // Logic to find policies due for renewal
        // For MVP, we'll just simulate finding some and sending reminders
        // In real implementation, we would query HEI Core or local policy cache
        
        // Example: Find reminders that haven't been sent
        var dueReminders = await _db.Set<RenewalReminder>()
            .Where(r => r.ReminderSentAt == default)
            .ToListAsync();

        foreach (var reminder in dueReminders)
        {
            await _notificationService.SendEmailAsync(reminder.UserId, "Policy Renewal Due", $"Your policy {reminder.PolicyId} is due on {reminder.DueDate}");
            reminder.ReminderSentAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<RenewalReminder>> GetRemindersForUserAsync(string userId)
    {
        return await _db.Set<RenewalReminder>()
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.DueDate)
            .ToListAsync();
    }
}
