using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForBudgetSubheading : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<BudgetSubheading>(p => p.BudgetHeadingId).CanFilter().CanSort();
        mapper.Property<BudgetSubheading>(p => p.Name).CanFilter().CanSort();
        mapper.Property<BudgetSubheading>(p => p.Code).CanFilter().CanSort();
        mapper.Property<BudgetSubheading>(p => p.Description).CanFilter().CanSort();
        mapper.Property<BudgetSubheading>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<BudgetSubheading>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
