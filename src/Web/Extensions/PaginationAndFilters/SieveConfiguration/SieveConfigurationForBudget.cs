using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForBudget : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Budget>(p => p.DepartmentId).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.Year).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.Quarter).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.TotalAmount).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.AllocatedAmount).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.RemainingAmount).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<Budget>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
