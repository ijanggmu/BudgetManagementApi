using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeemaEdgeApi.Extensions.Application;
using BeemaEdgeApi.Extensions.Cache;
using BeemaEdgeApi.Extensions.Cors;
using BeemaEdgeApi.Extensions.HealthCheck;
using BeemaEdgeApi.Extensions.Jobs;
using BeemaEdgeApi.Extensions.Logging.SeriLog;
using BeemaEdgeApi.Extensions.OpenApi;
using BeemaEdgeApi.Extensions.RateLimit;
using BeemaEdgeApi.Extensions.Refit;
using BeemaEdgeApi.Filters.ActionFilters;
using BeemaEdgeApi.Middleware;
using Data.Context;
using Data.Entities.Identity;
using Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.ConfigureSerilog();

    Log.Information("Type: {@type} Starting up application", "Api");

    builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add<CustomBadRequestCustomFilterAttribute>();
    });

    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = 10_000_000; // 10mb
    });

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 10_000_000; // 10mb
    });

    ConfigureConfiguration(builder);

    builder.Services.AddApplicationDatabase(builder.Configuration)
        .AddAuthenticationServices(builder.Configuration)
        .AddApplicationExtension()
        .AddApplicationIndividualServiceExtension()
        .AddApplicationCommonServiceExtension()
        .AddApplicationAdminServiceExtension()
        .AddApplicationConfigExtension()
        .AddApplicationCoreApiRefitServices(builder.Configuration)
        .AddTenancy()
        .AddTenantDomainServices()
        .AddCorsPolicy(builder.Configuration)
        .ConfigureOpenApi()
        .ConfigureJobs()
        .ConfigureHealthCheck(builder.Configuration)
        .ConfigureRateLimit(builder.Configuration)
        .AddCacheService(builder.Configuration)
        .AddExceptionHandler<GlobalExceptionHandler>()
        .AddProblemDetails()
        .Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        });

    var app = builder.Build();

    await RunDatabaseMigrationAsync(app);

    app.UseHttpsRedirection();
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
    });
    app.UseRateLimit();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseExceptionHandler();
    app.UseCorsPolicy();
    app.UseOpenApi(app.Configuration);
    app.UseJobDashboard(app.Configuration);
    app.UseResponseCompression();
    app.UseRouting();
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseTenantResolution();
    app.UseAuthorization();
    app.UseRequestLogging();

    app.MapControllers();
    app.MapHealthCheckEndPoint();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Error("Type: {@type} Application start-up failed Message: {@message} Exception: @{exception}", "Api",
        ex.Message, ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void ConfigureConfiguration(WebApplicationBuilder builder)
{
    builder.Configuration
        .AddEnvironmentVariables(prefix: "ASPNETCORE_");

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Common settings
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
            optional: true) // Environment-specific settings
        .AddEnvironmentVariables();

    var userSecretId = Environment.GetEnvironmentVariable("ASPNETCORE_API_USER_SECRETS_ID");

    if (!string.IsNullOrEmpty(userSecretId))
        builder.Configuration.AddUserSecrets(userSecretId);
}

static async Task RunDatabaseMigrationAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }

        var auditContext = scope.ServiceProvider.GetRequiredService<AuditDataContext>();
        if ((await auditContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await auditContext.Database.MigrateAsync();
        }

        var hangfireContext = scope.ServiceProvider.GetRequiredService<HangfireDataContext>();
        if ((await hangfireContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await hangfireContext.Database.MigrateAsync();
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        await RoleSeeder.SeedData(roleManager);
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await UserSeeder.SeedData(context, userManager);
        await MenuPermissionSeeder.SeedPermissionsForRole(context);
        await CountriesSeeder.SeedData(context);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during migration");
        throw;
    }
}
