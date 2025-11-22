using Data.Context;
using Data.Entities.Audit;
using Data.Entities.Audit.UserActivites;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BeemaEdgeApi.Utilities.Audits;
using BeemaEdgeApi.Utilities.UserActivities;

namespace BeemaEdgeApi.Extensions.Application;

public static class ApplicationDatabaseExtension
{
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, IConfiguration _configuration)
    {
        services.AddSingleton(_ => System.Threading.Channels.Channel.CreateUnbounded<SaveChangesAudit>());

        services.AddSingleton<AuditingInterceptor>();

        services.AddDbContext<ApplicationDataContext>((sp, opt) =>
        {
            var resolver = sp.GetRequiredService<Data.Infrastructure.ITenantConnectionResolver>();
            opt.UseNpgsql(resolver.ResolveDefaultConnection());
            var interceptor = sp.GetService<AuditingInterceptor>();
            opt.AddInterceptors(interceptor);
        });

        services.AddDbContextFactory<ApplicationDataContext>((sp, opt) =>
        {
            var resolver = sp.GetRequiredService<Data.Infrastructure.ITenantConnectionResolver>();
            opt.UseNpgsql(resolver.ResolveDefaultConnection());
            var interceptor = sp.GetService<AuditingInterceptor>();
            opt.AddInterceptors(interceptor);
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<AuditDataContext>((sp, opt) => opt.UseNpgsql(_configuration.GetConnectionString("AuditDefaultConnection")));

        services.AddHostedService<AuditDbStorageWriterBackgroundWorker>();

        services.AddDbContext<AuditDataContext>(opt => opt.UseNpgsql(_configuration.GetConnectionString("AuditDefaultConnection")));

        services.AddDbContext<HangfireDataContext>(options => options.UseNpgsql(_configuration.GetConnectionString("HangFireDefaultConnection")));

        services.AddHangfire(x => x.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(_configuration.GetConnectionString("HangFireDefaultConnection"))));


        services.AddSingleton(_ => System.Threading.Channels.Channel.CreateUnbounded<UserActivity>());
        services.AddHostedService<RequestLoggingDbWriterBackgroundWorker>();

        services.AddScoped<Data.Infrastructure.ITenantConnectionResolver, Data.Infrastructure.TenantConnectionResolver>();
        return services;
    }
}
