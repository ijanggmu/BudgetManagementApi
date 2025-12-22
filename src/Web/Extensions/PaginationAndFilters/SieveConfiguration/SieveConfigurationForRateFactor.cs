using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForRateFactor : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<RateFactor>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<RateFactor>(p => p.ProductId)
           .CanFilter()
           .CanSort();

        mapper.Property<RateFactor>(p => p.Key)
          .CanFilter()
          .CanSort();

        mapper.Property<RateFactor>(p => p.DataType)
          .CanFilter()
          .CanSort();

        mapper.Property<RateFactor>(p => p.AllowedValuesJson)
          .CanFilter()
          .CanSort();

        mapper.Property<RateFactor>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<RateFactor>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RateFactor>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<RateFactor>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RateFactor>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

