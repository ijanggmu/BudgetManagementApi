using Data.Entities.Audit.UserActivites;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForUserActivity : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<UserActivity>(p => p.At)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.EndAt)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.IpAddress)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.RequestHost)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.ResponseStatusCode)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.CorrelationId)
           .CanFilter()
           .CanSort();

        mapper.Property<UserActivity>(p => p.RequestPath)
           .CanFilter()
           .CanSort();
    }
}
