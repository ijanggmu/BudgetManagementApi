using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForApprovalConfig : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ApprovalConfig>(p => p.DepartmentId).CanFilter().CanSort();
        mapper.Property<ApprovalConfig>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<ApprovalConfig>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
