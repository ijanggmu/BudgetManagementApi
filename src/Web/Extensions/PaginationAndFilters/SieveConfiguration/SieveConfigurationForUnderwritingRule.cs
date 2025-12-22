using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForUnderwritingRule : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<UnderwritingRule>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<UnderwritingRule>(p => p.ProductId)
           .CanFilter()
           .CanSort();

        mapper.Property<UnderwritingRule>(p => p.Expression)
          .CanFilter()
          .CanSort();

        mapper.Property<UnderwritingRule>(p => p.Message)
          .CanFilter()
          .CanSort();

        mapper.Property<UnderwritingRule>(p => p.EffectiveFrom)
          .CanFilter()
          .CanSort();

        mapper.Property<UnderwritingRule>(p => p.EffectiveTo)
          .CanFilter()
          .CanSort();

        mapper.Property<UnderwritingRule>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<UnderwritingRule>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<UnderwritingRule>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<UnderwritingRule>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<UnderwritingRule>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

