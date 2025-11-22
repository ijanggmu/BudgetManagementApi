using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Context;
using Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace BeemaEdgeApi.Hosted;

public class TenantProvisioningHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TenantProvisioningHostedService> _logger;
    private readonly TenancyOptions _options;

    public TenantProvisioningHostedService(IServiceProvider services, ILogger<TenantProvisioningHostedService> logger, IOptions<TenancyOptions> options)
    {
        _services = services;
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.Strategy == TenantStorageStrategy.SharedTable)
        {
            return;
        }

        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();

        var tenants = await db.Set<Data.Entities.Tenant.Tenant>()
            .AsNoTracking()
            .Where(t => t.IsActive)
            .Select(t => t.Slug)
            .ToListAsync(cancellationToken);

        foreach (var slug in tenants)
        {
            try
            {
                using var tenantScope = _services.CreateScope();
                var ctx = tenantScope.ServiceProvider.GetRequiredService<ITenantContext>();
                ctx.Slug = slug;

                var runner = tenantScope.ServiceProvider.GetRequiredService<ITenantMigrationRunner>();
                await runner.MigrateCurrentTenantAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed provisioning for tenant {Slug}", slug);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}


