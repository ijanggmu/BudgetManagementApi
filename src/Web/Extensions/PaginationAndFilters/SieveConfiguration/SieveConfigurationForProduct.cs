using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForProduct : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Product>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.Code)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.Name)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.Description)
          .CanFilter()
          .CanSort();

        mapper.Property<Product>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Product>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Product>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}
