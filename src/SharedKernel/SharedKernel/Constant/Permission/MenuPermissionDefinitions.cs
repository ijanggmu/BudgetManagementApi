namespace SharedKernel.Constant.Permission;

/// <summary>
/// Centralized permission definitions using the helper for maintainability
/// All permissions are generated here using PermissionHelper
/// </summary>
public static class MenuPermissionDefinitions
{
    // Dashboard (read-only, no CRUD)
    public static readonly MenuPermissionSet Dashboard = PermissionHelper.GeneratePermissions(
        "1", "Dashboard", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Administration Section
    public static readonly MenuPermissionSet Administration = PermissionHelper.GeneratePermissions(
        "18", "Administration", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Tenants = PermissionHelper.GeneratePermissions("18-12", "Tenants");
    public static readonly MenuPermissionSet Roles = PermissionHelper.GeneratePermissions("18-2", "Roles");
    public static readonly MenuPermissionSet AdminManagement = PermissionHelper.GeneratePermissions("18-4", "Admin Management");

    // Sales & Marketing Section
    public static readonly MenuPermissionSet SalesMarketing = PermissionHelper.GeneratePermissions(
        "19", "Sales & Marketing", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet MarketingExecutives = PermissionHelper.GeneratePermissions("19-13", "Marketing Executives");
    public static readonly MenuPermissionSet AdminLeads = PermissionHelper.GeneratePermissions("19-5", "Leads");
    public static readonly MenuPermissionSet AdminQuotations = PermissionHelper.GeneratePermissions("19-6", "Quotations");

    // Operations Section
    public static readonly MenuPermissionSet Operations = PermissionHelper.GeneratePermissions(
        "20", "Operations", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet PremiumCalculation = PermissionHelper.GeneratePermissions(
        "20-9", "Premium Calculation", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet PremiumOverview = PermissionHelper.GeneratePermissions(
        "20-9-10", "Premium Overview", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet PremiumConfigurations = PermissionHelper.GeneratePermissions("20-9-11", "Premium Configurations");
    public static readonly MenuPermissionSet Notifications = PermissionHelper.GeneratePermissions(
        "20-14", "Notifications", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Administration - Branch and Designation
    public static readonly MenuPermissionSet Branch = PermissionHelper.GeneratePermissions("18-14", "Branch");
    public static readonly MenuPermissionSet Designation = PermissionHelper.GeneratePermissions("18-15", "Designation");

    // Operations - Entity Settings
    public static readonly MenuPermissionSet EntitySettings = PermissionHelper.GeneratePermissions(
        "20-18", "Entity Settings", includeCreate: false, includeUpdate: true, includeDelete: false, includeExport: false);

    // Administration - Additional
    public static readonly MenuPermissionSet Branding = PermissionHelper.GeneratePermissions(
        "18-16", "Branding", includeCreate: false, includeUpdate: true, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet Profile = PermissionHelper.GeneratePermissions(
        "18-17", "Profile", includeCreate: false, includeUpdate: true, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet Password = PermissionHelper.GeneratePermissions(
        "18-18", "Password", includeCreate: false, includeUpdate: true, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet TwoFactor = PermissionHelper.GeneratePermissions(
        "18-19", "Two Factor", includeCreate: false, includeUpdate: true, includeDelete: false, includeExport: false);

    // Operations - Additional
    public static readonly MenuPermissionSet Attendance = PermissionHelper.GeneratePermissions(
        "20-19", "Attendance", includeCreate: true, includeUpdate: false, includeDelete: false, includeExport: true);
    public static readonly MenuPermissionSet Reporting = PermissionHelper.GeneratePermissions(
        "20-20", "Reporting", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Sales & Marketing - Marketing Executive specific
    public static readonly MenuPermissionSet MarketingExecutiveLeads = PermissionHelper.GeneratePermissions(
        "19-14", "Marketing Executive Leads", includeCreate: true, includeUpdate: true, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet MarketingExecutiveQuotations = PermissionHelper.GeneratePermissions(
        "19-15", "Marketing Executive Quotations", includeCreate: true, includeUpdate: true, includeDelete: true, includeExport: false);
    public static readonly MenuPermissionSet CommonAttendance = PermissionHelper.GeneratePermissions(
        "19-16", "Common Attendance", includeCreate: true, includeUpdate: false, includeDelete: false, includeExport: false);

    // System Section
    public static readonly MenuPermissionSet System = PermissionHelper.GeneratePermissions(
        "21", "System", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Logs = PermissionHelper.GeneratePermissions(
        "21-15", "Logs", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet SystemLog = PermissionHelper.GeneratePermissions(
        "21-15-16", "System Log", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Config = PermissionHelper.GeneratePermissions(
        "21-17", "Config", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Common Utilities (for lookup endpoints)
    public static readonly MenuPermissionSet CommonUtilities = PermissionHelper.GeneratePermissions(
        "22-1", "Common Utilities", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet FileUpload = PermissionHelper.GeneratePermissions(
        "22-2", "File Upload", includeCreate: true, includeUpdate: false, includeDelete: false, includeExport: false);

    /// <summary>
    /// Gets a permission set by name (for backward compatibility and easy access)
    /// </summary>
    public static MenuPermissionSet GetPermissionSet(string name)
    {
        return name switch
        {
            nameof(Dashboard) => Dashboard,
            nameof(Administration) => Administration,
            nameof(Tenants) => Tenants,
            nameof(Roles) => Roles,
            nameof(AdminManagement) => AdminManagement,
            nameof(SalesMarketing) => SalesMarketing,
            nameof(MarketingExecutives) => MarketingExecutives,
            nameof(AdminLeads) => AdminLeads,
            nameof(AdminQuotations) => AdminQuotations,
            nameof(Operations) => Operations,
            nameof(PremiumCalculation) => PremiumCalculation,
            nameof(PremiumOverview) => PremiumOverview,
            nameof(PremiumConfigurations) => PremiumConfigurations,
            nameof(Notifications) => Notifications,
            nameof(System) => System,
            nameof(Logs) => Logs,
            nameof(SystemLog) => SystemLog,
            nameof(Config) => Config,
            nameof(Branch) => Branch,
            nameof(Designation) => Designation,
            nameof(EntitySettings) => EntitySettings,
            nameof(Branding) => Branding,
            nameof(Profile) => Profile,
            nameof(Password) => Password,
            nameof(TwoFactor) => TwoFactor,
            nameof(Attendance) => Attendance,
            nameof(Reporting) => Reporting,
            nameof(MarketingExecutiveLeads) => MarketingExecutiveLeads,
            nameof(MarketingExecutiveQuotations) => MarketingExecutiveQuotations,
            nameof(CommonAttendance) => CommonAttendance,
            nameof(CommonUtilities) => CommonUtilities,
            nameof(FileUpload) => FileUpload,
            _ => null
        };
    }
}

