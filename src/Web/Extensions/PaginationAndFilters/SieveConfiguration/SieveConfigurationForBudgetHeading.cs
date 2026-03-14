using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForBudgetHeading : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<BudgetHeading>(p => p.Name).CanFilter().CanSort();
        mapper.Property<BudgetHeading>(p => p.Code).CanFilter().CanSort();
        mapper.Property<BudgetHeading>(p => p.Description).CanFilter().CanSort();
        mapper.Property<BudgetHeading>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<BudgetHeading>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
