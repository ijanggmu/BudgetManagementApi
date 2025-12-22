using Data.Entities.Draft;
using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPolicyDraft : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Tenant>(p => p.CreatedOn)
            .CanFilter()
            .CanSort();

        mapper.Property<Tenant>(p => p.IsActive)
            .CanFilter()
            .CanSort();

        mapper.Property<Tenant>(p => p.Slug)
            .CanFilter();

        mapper.Property<Tenant>(p => p.Name)
            .CanFilter();

    }
}

