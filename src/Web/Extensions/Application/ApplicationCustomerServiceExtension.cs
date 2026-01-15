using AdminPortalApi.Controllers.V1.SystemLog;
using Business.AdminPortalApi.AdminPassword;
using Business.AdminPortalApi.Auth;
using Business.AdminPortalApi.Claim;
using Business.AdminPortalApi.CMSCustomer;
using Business.AdminPortalApi.Profile;
using Business.BeemaEdgeApi.AccountValidation;
using Business.BeemaEdgeApi.Auth;
using Business.BeemaEdgeApi.Bank;
using Business.BeemaEdgeApi.Branch;
using Business.BeemaEdgeApi.CustomerPassword;
using Business.BeemaEdgeApi.Dashboard;
using Business.BeemaEdgeApi.Kyc;
using Business.BeemaEdgeApi.Permission;
using Business.BeemaEdgeApi.Profile;
using Business.BeemaEdgeApi.Province;
using Business.BeemaEdgeApi.Registration;
using Business.BeemaEdgeApi.Role;
using Business.BeemaEdgeApi.TwoFactor;
using Business.Common.Country;
using Business.Common.File;
using Business.Common.Hmac;
using Business.Common.StringCipher;
using Business.Common.Totp;
using Microsoft.Extensions.DependencyInjection;

namespace BeemaEdgeApi.Extensions.Application;

public static class ApplicationCustomerServiceExtension
{
    public static IServiceCollection AddApplicationIndividualServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ISystemLogService, SystemLogService>();
        services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
        services.AddScoped<ICustomerAccountValidationService, CustomerAccountValidationService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();
        services.AddScoped<ICustomerPasswordService, CustomerPasswordService>();
        services.AddScoped<ICustomerTwoFactorService, CustomerTwoFactorService>();
        services.AddScoped<ICustomerKycService, CustomerKycService>();
        services.AddScoped<ICustomerProfileService, CustomerProfileService>();
        services.AddScoped<IMenuPermissionService, MenuPermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITotpService, TotpService>();
        services.AddScoped<StringCipherService>();
        services.AddScoped<ICustomerDashboardService, CustomerDashboardService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddHttpClient();

        return services;
    }
    public static IServiceCollection AddApplicationCommonServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IHmacValidatorService, HmacValidatorService>();
        return services;
    }
    public static IServiceCollection AddApplicationAdminServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICmsCustomerService, CmsCustomerService>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminProfileService, AdminProfileService>();
        services.AddScoped<IAdminPasswordService, AdminPasswordService>();
        services.AddScoped<Business.AdminPortalApi.Gateway.IGatewayConfigurationService, Business.AdminPortalApi.Gateway.GatewayConfigurationService>();
        services.AddScoped<Business.AdminPortalApi.Dashboard.IDashboardService, Business.AdminPortalApi.Dashboard.DashboardService>();
        services.AddScoped<Business.AdminPortalApi.Notification.INotificationService, Business.AdminPortalApi.Notification.NotificationService>();
        services.AddScoped<Business.AdminPortalApi.Notification.INotificationSender, BeemaEdgeApi.Services.NotificationSenderService>();
        services.AddScoped<Business.AdminPortalApi.Noticeboard.INoticeboardService, Business.AdminPortalApi.Noticeboard.NoticeboardService>();

        return services;
    }
}
