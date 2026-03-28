using Data.Entities.Identity;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForRole : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ApplicationRole>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<ApplicationRole>(p => p.Name)
           .CanFilter()
           .CanSort();

        mapper.Property<ApplicationRole>(p => p.Description)
           .CanFilter()
           .CanSort();

        mapper.Property<ApplicationRole>(p => p.TenantId)
           .CanFilter()
           .CanSort();
    }
}
