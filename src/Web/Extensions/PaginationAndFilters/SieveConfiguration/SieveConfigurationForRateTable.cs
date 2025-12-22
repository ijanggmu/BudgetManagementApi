using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForRateTable : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<RateTable>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<RateTable>(p => p.ProductId)
           .CanFilter()
           .CanSort();

        mapper.Property<RateTable>(p => p.Version)
          .CanFilter()
          .CanSort();

        mapper.Property<RateTable>(p => p.EffectiveFrom)
          .CanFilter()
          .CanSort();

        mapper.Property<RateTable>(p => p.EffectiveTo)
          .CanFilter()
          .CanSort();

        mapper.Property<RateTable>(p => p.SourceUri)
          .CanFilter()
          .CanSort();

        mapper.Property<RateTable>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<RateTable>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RateTable>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<RateTable>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RateTable>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

