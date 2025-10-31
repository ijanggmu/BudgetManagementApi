using Business.Common.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Config;

namespace BeemaEdgeApi.Extensions.Cache;

public static class Extensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions = configuration.GetSection("CacheOptions").Get<CacheOptions>();

        // Check if cache is enabled and cache type is in-memory
        if (cacheOptions.IsEnabled && cacheOptions.CacheType == nameof(CacheType.InMemory))
        {
            services.AddOptions<CacheOptions>().BindConfiguration(nameof(CacheOptions));
            services.AddMemoryCache(memoryOptions => memoryOptions.TrackStatistics = true);
            services.AddScoped<ICacheService, MemoryCacheService>();
        }

        return services;
    }
    public enum CacheType
    {
        InMemory,
        Redis
    }
}
