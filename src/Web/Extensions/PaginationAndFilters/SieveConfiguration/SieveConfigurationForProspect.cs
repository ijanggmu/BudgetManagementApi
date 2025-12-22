using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForProspect : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Prospect>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Prospect>(p => p.PrimaryContactId)
           .CanFilter()
           .CanSort();

        mapper.Property<Prospect>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Prospect>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Prospect>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Prospect>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Prospect>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

