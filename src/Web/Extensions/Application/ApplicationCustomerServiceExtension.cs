using System;
using System.Collections.Generic;
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
using Business.BeemaEdgeApi.PolicyDraftService;
using Business.BeemaEdgeApi.Profile;
using Business.BeemaEdgeApi.Province;
using Business.BeemaEdgeApi.Registration;
using Business.BeemaEdgeApi.Role;
using Business.BeemaEdgeApi.TwoFactor;
using Business.Common.Country;
using Business.Common.File;
using Business.Common.Hmac;
using Business.Common.PaymentGatewayRedirection;
using Business.Common.Policy;
using Business.Common.PolicyCalculator;
using Business.Common.PolicyPayment.PaymentGateway;
using Business.Common.PolicyPayment.PaymentGateway.Esewa;
using Business.Common.PolicyPayment.PaymentGateway.Khalti;
using Business.Common.PolicyPayment.Purchase;
using Business.Common.StringCipher;
using Business.Common.Totp;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.SystemEnum.Payment;
using static BeemaEdgeApi.Controllers.V1.Common.Class.ClassController;

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
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ICustomerDashboardService, CustomerDashboardService>();
        services.AddScoped<IPolicyDraftService, PolicyDraftService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddHttpClient();

        return services;
    }
    public static IServiceCollection AddApplicationCommonServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<IPolicyCalculatorService, PolicyCalculatorService>();
        services.AddScoped<IPolicyPremiumCalculatorService, PolicyPremiumCalculatorService>();
        services.AddScoped<IPolicyAcknowlegeService, PolicyAcknowlegeService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IHmacValidatorService, HmacValidatorService>();

        services.AddHttpClient<ESewaService>();
        services.AddScoped<ESewaPaymentGatewayService>();
        services.AddScoped<KhaltiPaymentGatewayService>();
        services.AddScoped<IPaymentGatewayService, ESewaPaymentGatewayService>(); // optional default
        services.AddScoped<IPaymentGatewayRedirectionService, PaymentGatewayRedirectionService>(); // optional default
        services.AddScoped(provider =>
        {
            var dict = new Dictionary<string, IPaymentGatewayService>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(PaymentGateway.Esewa)] = provider.GetRequiredService<ESewaPaymentGatewayService>(),
                [nameof(PaymentGateway.Khalti)] = provider.GetRequiredService<KhaltiPaymentGatewayService>()
            };
            return dict;
        });

        return services;
    }
    public static IServiceCollection AddApplicationAdminServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICmsCustomerService, CmsCustomerService>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminProfileService, AdminProfileService>();
        services.AddScoped<IAdminPasswordService, AdminPasswordService>();
        services.AddScoped<Business.AdminPortalApi.Gateway.IGatewayConfigurationService, Business.AdminPortalApi.Gateway.GatewayConfigurationService>();

        return services;
    }
}
