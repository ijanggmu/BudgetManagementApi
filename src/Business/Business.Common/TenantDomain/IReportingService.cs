using System.Threading.Tasks;
using System.Collections.Generic;

namespace Business.Common.TenantDomain;

public interface IReportingService
{
    Task<Dictionary<string, object>> GetDashboardStatsAsync(string userId);
}
