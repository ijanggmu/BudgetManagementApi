using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForCoverage : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Coverage>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.ProductId)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.Code)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.Name)
          .CanFilter()
          .CanSort();

        mapper.Property<Coverage>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Coverage>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Coverage>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

