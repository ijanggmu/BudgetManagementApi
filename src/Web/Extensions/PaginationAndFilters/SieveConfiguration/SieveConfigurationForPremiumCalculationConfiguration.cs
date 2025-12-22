using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPremiumCalculationConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PremiumCalculationConfiguration>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.PortfolioAlias)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.FiscalYear)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.Version)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.EffectiveFrom)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.EffectiveTo)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.IsActive)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.Description)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.CalculationEngineType)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationConfiguration>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

