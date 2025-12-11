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

    // System Section
    public static readonly MenuPermissionSet System = PermissionHelper.GeneratePermissions(
        "21", "System", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);
    
    public static readonly MenuPermissionSet Logs = PermissionHelper.GeneratePermissions(
        "21-15", "Logs", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);
    
    public static readonly MenuPermissionSet SystemLog = PermissionHelper.GeneratePermissions(
        "21-15-16", "System Log", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);
    
    public static readonly MenuPermissionSet Config = PermissionHelper.GeneratePermissions(
        "21-17", "Config", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

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
            _ => null
        };
    }
}

