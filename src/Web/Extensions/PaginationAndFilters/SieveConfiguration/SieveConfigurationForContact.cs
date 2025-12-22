using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForContact : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Contact>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Contact>(p => p.FullName)
           .CanFilter()
           .CanSort();

        mapper.Property<Contact>(p => p.Email)
          .CanFilter()
          .CanSort();

        mapper.Property<Contact>(p => p.Phone)
          .CanFilter()
          .CanSort();

        mapper.Property<Contact>(p => p.AddressId)
          .CanFilter()
          .CanSort();

        mapper.Property<Contact>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Contact>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Contact>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Contact>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Contact>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

