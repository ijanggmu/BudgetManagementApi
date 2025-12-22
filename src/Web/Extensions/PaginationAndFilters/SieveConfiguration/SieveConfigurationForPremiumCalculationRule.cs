using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPremiumCalculationRule : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PremiumCalculationRule>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.ConfigurationId)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.RuleName)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.RuleType)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.Expression)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.Condition)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.Priority)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.IsActive)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRule>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

