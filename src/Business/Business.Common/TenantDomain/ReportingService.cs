using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Business.Common.TenantDomain;

public class ReportingService : IReportingService
{
    private readonly ApplicationDataContext _db;

    public ReportingService(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<string, object>> GetDashboardStatsAsync(string userId)
    {
        // Aggregate data for dashboard
        var stats = new Dictionary<string, object>();

        var leadsCount = await _db.Set<Lead>().CountAsync(); // Filter by user/region if needed
        var quotesCount = await _db.Set<Quotation>().CountAsync();
        var renewalsDue = await _db.Set<RenewalReminder>().CountAsync(r => r.UserId == userId && r.ReminderSentAt == default);

        stats.Add("TotalLeads", leadsCount);
        stats.Add("TotalQuotations", quotesCount);
        stats.Add("RenewalsDue", renewalsDue);

        return stats;
    }
}
