using Business.AdminPortalApi.PdfGeneration;
using Business.Common.TenantDomain;
using Microsoft.Extensions.DependencyInjection;

namespace BeemaEdgeApi.Extensions.Application;

public static class TenantDomainServicesExtension
{
    public static IServiceCollection AddTenantDomainServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IQuotationPdfService, QuotationPdfService>();
        services.AddScoped<IBrandingService, BrandingService>();
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IQuotationNumberGenerator, QuotationNumberGenerator>();
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddScoped<ITenantAdminService, TenantAdminService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IRenewalService, RenewalService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationUserService, NotificationUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITenantAuthService, TenantAuthService>();
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IFodoService, FodoService>();
        services.AddScoped<IFodoAuthService, FodoAuthService>();
        services.AddScoped<IFodoRegistrationService, FodoRegistrationService>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}


