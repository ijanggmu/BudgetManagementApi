using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPremiumFormula : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PremiumFormula>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumFormula>(p => p.ProductId)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumFormula>(p => p.Version)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumFormula>(p => p.Expression)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumFormula>(p => p.EffectiveFrom)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumFormula>(p => p.EffectiveTo)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumFormula>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumFormula>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumFormula>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumFormula>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumFormula>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

