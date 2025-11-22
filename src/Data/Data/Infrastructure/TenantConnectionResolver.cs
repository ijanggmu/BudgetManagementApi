using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace Data.Infrastructure;

public interface ITenantConnectionResolver
{
    string ResolveDefaultConnection();
}

public class TenantConnectionResolver : ITenantConnectionResolver
{
    private readonly IConfiguration _config;
    private readonly ITenantContext _tenant;
    private readonly TenancyOptions _options;

    public TenantConnectionResolver(IConfiguration config, ITenantContext tenant, IOptions<TenancyOptions> options)
    {
        _config = config;
        _tenant = tenant;
        _options = options.Value;
    }

    public string ResolveDefaultConnection()
    {
        if (_options.Strategy != TenantStorageStrategy.SeparateDatabase)
            return _config.GetConnectionString("DefaultConnection");

        var slug = _tenant.Slug;
        if (string.IsNullOrWhiteSpace(slug))
            return _config.GetConnectionString("DefaultConnection");

        var key = $"ConnectionStrings:Tenants:{slug}";
        var conn = _config[key];
        return string.IsNullOrWhiteSpace(conn) ? _config.GetConnectionString("DefaultConnection") : conn;
    }
}


