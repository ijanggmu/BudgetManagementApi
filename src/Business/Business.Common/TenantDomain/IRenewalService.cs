using System.Threading.Tasks;
using Data.Entities.Tenant;

namespace Business.Common.TenantDomain;

public interface IRenewalService
{
    Task CheckAndSendRemindersAsync();
    Task<IEnumerable<RenewalReminder>> GetRemindersForUserAsync(string userId);
}
