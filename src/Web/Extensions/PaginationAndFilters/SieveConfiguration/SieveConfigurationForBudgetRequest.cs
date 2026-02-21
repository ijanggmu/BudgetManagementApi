using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForBudgetRequest : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<BudgetRequest>(p => p.DepartmentId).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.UserId).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.Amount).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.Purpose).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.Status).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.RequestedDate).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<BudgetRequest>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
