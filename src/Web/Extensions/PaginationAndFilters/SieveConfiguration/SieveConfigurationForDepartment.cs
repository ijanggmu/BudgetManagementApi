using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForDepartment : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Department>(p => p.Name).CanFilter().CanSort();
        mapper.Property<Department>(p => p.Description).CanFilter().CanSort();
        mapper.Property<Department>(p => p.IsActive).CanFilter().CanSort();
        mapper.Property<Department>(p => p.TenantId).CanFilter().CanSort();
        mapper.Property<Department>(p => p.CreatedOn).CanFilter().CanSort();
    }
}
