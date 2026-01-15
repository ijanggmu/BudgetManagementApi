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

        var leadsCount = 123;
        var quotesCount = 234;
        var renewalsDue = 12313;

        stats.Add("TotalLeads", leadsCount);
        stats.Add("TotalQuotations", quotesCount);
        stats.Add("RenewalsDue", renewalsDue);

        return stats;
    }
}
