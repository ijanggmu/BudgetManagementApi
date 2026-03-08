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
    public static readonly MenuPermissionSet Menu = PermissionHelper.GeneratePermissions("18-3", "Menu", includeCreate: false, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet AdminManagement = PermissionHelper.GeneratePermissions("18-4", "Admin Management");

    // Operations Section
    public static readonly MenuPermissionSet Operations = PermissionHelper.GeneratePermissions(
        "20", "Operations", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Notifications = PermissionHelper.GeneratePermissions(
        "20-14", "Notifications", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Administration - Branch and Designation
    public static readonly MenuPermissionSet Branch = PermissionHelper.GeneratePermissions("18-14", "Branch");
    public static readonly MenuPermissionSet Designation = PermissionHelper.GeneratePermissions("18-15", "Designation");

    // Operations - Entity Settings
    public static readonly MenuPermissionSet Entity = PermissionHelper.GeneratePermissions(
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

    // System Section
    public static readonly MenuPermissionSet System = PermissionHelper.GeneratePermissions(
        "21", "System", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Logs = PermissionHelper.GeneratePermissions(
        "21-15", "Logs", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet SystemLog = PermissionHelper.GeneratePermissions(
        "21-15-16", "System Log", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    public static readonly MenuPermissionSet Config = PermissionHelper.GeneratePermissions(
        "21-17", "Config", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);

    // Operations - Notice Board (moved from System)
    public static readonly MenuPermissionSet NoticeBoard = PermissionHelper.GeneratePermissions("20-22", "Notice Board", includeExport: false);

    // Operations - Gateway Configurations
    public static readonly MenuPermissionSet EmailGateway = PermissionHelper.GeneratePermissions("20-23", "Email Gateway");
    public static readonly MenuPermissionSet SmsGateway = PermissionHelper.GeneratePermissions("20-24", "SMS Gateway");

    // Common Utilities (for lookup endpoints)
    public static readonly MenuPermissionSet CommonUtilities = PermissionHelper.GeneratePermissions(
        "22-1", "Common Utilities", includeCreate: false, includeUpdate: false, includeDelete: false, includeExport: false);
    public static readonly MenuPermissionSet FileUpload = PermissionHelper.GeneratePermissions(
        "22-2", "File Upload", includeCreate: true, includeUpdate: false, includeDelete: false, includeExport: false);

    // Budget Management (23-xx)
    public static readonly MenuPermissionSet Budget = PermissionHelper.GeneratePermissions("23-1", "Budget");
    public static readonly MenuPermissionSet Department = PermissionHelper.GeneratePermissions("23-4", "Department");
    public static readonly MenuPermissionSet ApprovalConfig = PermissionHelper.GeneratePermissions("23-5", "Approval Config");
    public static readonly MenuPermissionSet BudgetReport = PermissionHelper.GeneratePermissions(
        "23-7", "Budget Report", includeCreate: false, includeUpdate: false, includeDelete: false);
    public static readonly MenuPermissionSet BudgetHeadings = PermissionHelper.GeneratePermissions("23-9", "Budget Headings");

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
            nameof(Menu) => Menu,
            nameof(AdminManagement) => AdminManagement,
            nameof(Operations) => Operations,
            nameof(Notifications) => Notifications,
            nameof(System) => System,
            nameof(Logs) => Logs,
            nameof(SystemLog) => SystemLog,
            nameof(Config) => Config,
            nameof(Branch) => Branch,
            nameof(Designation) => Designation,
            nameof(Entity) => Entity,
            nameof(Branding) => Branding,
            nameof(Profile) => Profile,
            nameof(Password) => Password,
            nameof(TwoFactor) => TwoFactor,
            nameof(NoticeBoard) => NoticeBoard,
            nameof(EmailGateway) => EmailGateway,
            nameof(SmsGateway) => SmsGateway,
            nameof(CommonUtilities) => CommonUtilities,
            nameof(FileUpload) => FileUpload,
            nameof(Budget) => Budget,
            nameof(Department) => Department,
            nameof(ApprovalConfig) => ApprovalConfig,
            nameof(BudgetReport) => BudgetReport,
            nameof(BudgetHeadings) => BudgetHeadings,
            _ => null
        };
    }
}

