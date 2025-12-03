using Data.Entities.CustomerEntity;
using Data.Entities.PolicyE2e;
using Sieve.Services;

namespace BeemaEdgeApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPolicyDraft : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PolicyDraft>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();
    }
}
public class SieveConfigurationForCustomer : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {

        mapper.Property<Customer>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

    }
}
