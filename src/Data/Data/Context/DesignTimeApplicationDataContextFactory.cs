using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace Data.Context;

/// <summary>
/// Used by EF Core tools at design time (e.g. migrations) when the host is not available.
/// </summary>
public class DesignTimeApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (basePath.EndsWith("Data"))
            basePath = Path.Combine(basePath, "..", "..");
        else if (!basePath.Contains("Web") && !basePath.Contains("BeemaEdge"))
            basePath = Path.Combine(basePath, "src", "Web");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Dev.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=db_beema_core;Username=postgres;Password=postgres;";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var stubUserProfile = new DesignTimeUserProfileService();
        var stubTenantContext = new TenantContext();
        var stubSchemaProvider = new DesignTimeTenantSchemaProvider();
        var tenancyOptions = Microsoft.Extensions.Options.Options.Create(new TenancyOptions { Strategy = TenantStorageStrategy.SharedTable });

        return new ApplicationDataContext(
            optionsBuilder.Options,
            stubUserProfile,
            stubTenantContext,
            stubSchemaProvider,
            tenancyOptions);
    }

    private sealed class DesignTimeUserProfileService : global::Infrastructure.Common.UserProfile.IUserProfileService
    {
        public string GetUserId() => "design-time";
        public string GetUsername() => "design-time";
        public string GetUserTimeZone() => "UTC";
        public global::Infrastructure.Common.UserProfile.LoggingUserObject GetUser() => new() { UserId = "design-time", Username = "design-time", IpAddress = "" };
        public string GetIpAddress() => "";
        public string GetScheme() => "https";
        public string GetAccessToken() => "";
        public string GetRefreshToken() => "";
        public void SetAuthCookiesInClient(global::Models.Common.Token.TokenModel tokenModel, string username) { }
        public string GetRoleId() => "";
        public string GetRoleType() => "";
        public void RemoveAuthCookies(global::Microsoft.AspNetCore.Http.HttpResponse response) { }
        public void SetUser(string userId, string username) { }
    }

    private sealed class DesignTimeTenantSchemaProvider : global::Data.Infrastructure.ITenantSchemaProvider
    {
        public string GetSchemaOrNull() => null;
    }
}
