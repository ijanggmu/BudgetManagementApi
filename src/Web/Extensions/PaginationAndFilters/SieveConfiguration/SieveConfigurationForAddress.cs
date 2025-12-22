using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForAddress : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Address>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Address>(p => p.Line1)
           .CanFilter()
           .CanSort();

        mapper.Property<Address>(p => p.Line2)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.City)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.State)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.PostalCode)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.Country)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Address>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Address>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Address>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Address>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

