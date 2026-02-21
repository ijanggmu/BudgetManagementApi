using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForMemo : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Memo>(p => p.BudgetRequestId).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.RequestedBy).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.Amount).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.Purpose).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.Department).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.Status).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<Memo>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
