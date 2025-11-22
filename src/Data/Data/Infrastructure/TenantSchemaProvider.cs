using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace Data.Infrastructure;

public interface ITenantSchemaProvider
{
    string? GetSchemaOrNull();
}

public class TenantSchemaProvider : ITenantSchemaProvider
{
    private readonly ITenantContext _tenant;
    private readonly IOptions<TenancyOptions> _options;

    public TenantSchemaProvider(ITenantContext tenant, IOptions<TenancyOptions> options)
    {
        _tenant = tenant;
        _options = options;
    }

    public string? GetSchemaOrNull()
    {
        if (_options.Value.Strategy != TenantStorageStrategy.SeparateSchema) return null;
        var slug = _tenant.Slug;
        if (string.IsNullOrWhiteSpace(slug)) return null;
        var prefix = _options.Value.SchemaPrefix ?? "tenant_";
        return $"{prefix}{slug}".ToLowerInvariant();
    }
}


