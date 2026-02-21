using Business.AdminPortalApi.ExcelExport;
using Business.AdminPortalApi.PdfGeneration;
using Business.Common.PdfGeneration;
using Business.Common.TenantDomain;
using Microsoft.Extensions.DependencyInjection;
using PdfSharp.Fonts;

namespace BeemaEdgeApi.Extensions.Application;

public static class TenantDomainServicesExtension
{
    public static IServiceCollection AddTenantDomainServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        GlobalFontSettings.UseWindowsFontsUnderWindows = true;

        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IQuotationPdfService, QuotationPdfService>();
        services.AddScoped<IBrandingService, BrandingService>();
        services.AddScoped<ITenantAdminService, TenantAdminService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationUserService, NotificationUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITenantAuthService, TenantAuthService>();
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IEntitySettingsService, EntitySettingsService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IBudgetRequestService, BudgetRequestService>();
        services.AddScoped<IMemoService, MemoService>();
        services.AddScoped<IApprovalConfigService, ApprovalConfigService>();
        services.AddScoped<IUserSignatureService, UserSignatureService>();
        services.AddScoped<IBudgetReportService, BudgetReportService>();
        return services;
    }
}


