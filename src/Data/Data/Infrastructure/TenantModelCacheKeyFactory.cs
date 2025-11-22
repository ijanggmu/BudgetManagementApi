using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace Data.Infrastructure;

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    private readonly IOptions<TenancyOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantModelCacheKeyFactory(IHttpContextAccessor httpContextAccessor, IOptions<TenancyOptions> options)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    public object Create(DbContext context, bool designTime)
    {
        // Include schema name in cache key only when SeparateSchema strategy is used
        var suffix = string.Empty;
        if (_options.Value.Strategy == TenantStorageStrategy.SeparateSchema)
        {
            var schema = ResolveSchema();
            suffix = $":{schema}";
        }
        return (context.GetType(), designTime, suffix);
    }

    private string ResolveSchema()
    {
        var slug = _httpContextAccessor.HttpContext?.RequestServices?.GetService<ITenantContext>()?.Slug ?? "public";
        var prefix = _options.Value.SchemaPrefix ?? "tenant_";
        return $"{prefix}{slug}".ToLowerInvariant();
    }
}


