using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SharedKernel.Models.Tenancy;
using SharedKernel.Config;
using Data.Infrastructure;
using BeemaEdgeApi.Hosted;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BeemaEdgeApi.Extensions.Application;

public static class TenancyExtensions
{
    public static IServiceCollection AddTenancy(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddOptions<TenancyOptions>().BindConfiguration("Tenancy");
        services.TryAddScoped<ITenantSchemaProvider, TenantSchemaProvider>();
        services.TryAddScoped<ITenantSchemaEnsurer, TenantSchemaEnsurer>();
        services.TryAddScoped<ITenantMigrationRunner, TenantMigrationRunner>();
        services.TryAddSingleton<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
        services.AddHostedService<TenantProvisioningHostedService>();
        return services;
    }

    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<BeemaEdgeApi.Middleware.TenantResolutionMiddleware>();
    }
}


