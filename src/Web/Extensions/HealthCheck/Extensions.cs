using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BeemaEdgeApi.Extensions.HealthCheck;

public static class Extensions
{
    public static IServiceCollection ConfigureHealthCheck(this IServiceCollection services, IConfiguration _config)
    {
        services.AddHealthChecks()
               .AddNpgSql(_config.GetConnectionString("DefaultConnection"), name: "ApplicationDatabase")
               .AddNpgSql(_config.GetConnectionString("AuditDefaultConnection"), name: "AuditDatabase");

        return services;
    }
    public static IApplicationBuilder MapHealthCheckEndPoint(this WebApplication app)
    {
        app.MapHealthChecks("api/health", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        return app;

    }
}
