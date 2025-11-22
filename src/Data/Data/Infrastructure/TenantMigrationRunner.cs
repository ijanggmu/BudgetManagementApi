using System.Collections.Concurrent;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Tenancy;

namespace Data.Infrastructure;

public interface ITenantMigrationRunner
{
    Task MigrateCurrentTenantAsync();
}

public class TenantMigrationRunner : ITenantMigrationRunner
{
    private readonly IDbContextFactory<ApplicationDataContext> _factory;
    private readonly ITenantSchemaEnsurer _schemaEnsurer;
    private readonly ITenantContext _tenant;
    private static readonly ConcurrentDictionary<string, bool> _migrated = new();

    public TenantMigrationRunner(IDbContextFactory<ApplicationDataContext> factory, ITenantSchemaEnsurer schemaEnsurer, ITenantContext tenant)
    {
        _factory = factory;
        _schemaEnsurer = schemaEnsurer;
        _tenant = tenant;
    }

    public async Task MigrateCurrentTenantAsync()
    {
        var slug = _tenant.Slug ?? "__default";
        if (_migrated.ContainsKey(slug)) return;

        await using var ctx = await _factory.CreateDbContextAsync();
        await _schemaEnsurer.EnsureCurrentTenantSchemaAsync(ctx);
        await ctx.Database.MigrateAsync();
        _migrated.TryAdd(slug, true);
    }
}


