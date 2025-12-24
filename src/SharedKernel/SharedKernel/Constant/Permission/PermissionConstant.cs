namespace SharedKernel.Constant.Permission;

/// <summary>
/// Permission constants - Maintained for backward compatibility
/// New code should use MenuPermissionDefinitions for better maintainability
/// </summary>
public static class MenuPermissionConstant
{
    //format ParentMenuId-OperationType
    //format ParentMenuId-ChildMenuId-OperationType
    //format ParentMenuId-ChildMenuId-GrandChildMenuId-OperationType

    public enum MenuCount
    {
        Dashboard,
        Administration,
        SalesMarketing,
        Operations,
        System,
        Roles,
        Admins,
        AdminManagement,
        AdminLeads,
        AdminQuotations,
        Leads,
        Quotations,
        PremiumCalculation,
        Tenants,
        MarketingExecutives,
        Notifications,
        Logs,
        Config
    }

    //OperationType count = 44 //take next value
    public enum OperationType
    {
        View = 1,
        Create = 2,
        Update = 3,
        Delete = 4,
        Export = 5
    }

    // Menu Names (for backward compatibility)
    public const string DashboardName = "Dashboard";
    public const string AdministrationName = "Administration";
    public const string SalesMarketingName = "Sales & Marketing";
    public const string OperationsName = "Operations";
    public const string SystemName = "System";
    public const string RolesName = "Roles";
    public const string MenuName = "Menu";
    public const string AdminManagementName = "Admin Management";
    public const string TenantsName = "Tenants";
    public const string MarketingExecutivesName = "Marketing Executives";
    public const string AdminLeadsName = "Leads";
    public const string AdminQuotationsName = "Quotations";
    public const string PremiumCalculationName = "Premium Calculation";
    public const string PremiumOverviewName = "Overview";
    public const string PremiumConfigurationsName = "Configurations";
    public const string NotificationsName = "Notifications";
    public const string LogsName = "Logs";
    public const string SystemLogName = "System Log";
    public const string ConfigName = "Config";

    // Permission constants - Must be const for use in attributes
    // Dashboard
    public const string DashboardView = "1-1";
    public const string DashboardViewName = "Dashboard";

    // Administration
    public const string AdministrationView = "18-1";
    public const string AdministrationViewName = "Administration";
    
    public const string TenantsView = "18-12-1";
    public const string TenantsViewName = "Tenants View";
    public const string TenantsCreate = "18-12-2";
    public const string TenantsUpdate = "18-12-3";
    public const string TenantsDelete = "18-12-4";
    public const string TenantsExport = "18-12-5";
    
    public const string RolesView = "18-2-1";
    public const string RolesViewName = "Roles View";
    public const string RolesCreate = "18-2-2";
    public const string RolesUpdate = "18-2-3";
    public const string RolesDelete = "18-2-4";
    public const string RolesExport = "18-2-5";

    public const string MenuView = "18-3-1";
    public const string MenuViewName = "Menu View";
    public const string MenuUpdate = "18-3-3";

    public const string AdminManagementView = "18-4-1";
    public const string AdminManagementViewName = "Admin Management";
    public const string AdminManagementCreate = "18-4-2";
    public const string AdminManagementUpdate = "18-4-3";
    public const string AdminManagementDelete = "18-4-4";
    public const string AdminManagementExport = "18-4-5";

    // Sales & Marketing
    public const string SalesMarketingView = "19-1";
    public const string SalesMarketingViewName = "Sales & Marketing";
    
    public const string MarketingExecutivesView = "19-13-1";
    public const string MarketingExecutivesViewName = "Marketing Executives View";
    public const string MarketingExecutivesCreate = "19-13-2";
    public const string MarketingExecutivesUpdate = "19-13-3";
    public const string MarketingExecutivesDelete = "19-13-4";
    public const string MarketingExecutivesExport = "19-13-5";
    // Backward compatibility
    public const string FodoName = "Marketing Executives";
    public const string FodoView = "19-13-1";
    public const string FodoViewName = "Marketing Executives View";
    
    public const string AdminLeadsView = "19-5-1";
    public const string AdminLeadsViewName = "Leads View";
    public const string AdminLeadsCreate = "19-5-2";
    public const string AdminLeadsUpdate = "19-5-3";
    public const string AdminLeadsDelete = "19-5-4";
    public const string AdminLeadsExport = "19-5-5";
    
    public const string AdminQuotationsView = "19-6-1";
    public const string AdminQuotationsViewName = "Quotations View";
    public const string AdminQuotationsCreate = "19-6-2";
    public const string AdminQuotationsUpdate = "19-6-3";
    public const string AdminQuotationsDelete = "19-6-4";
    public const string AdminQuotationsExport = "19-6-5";

    // Operations
    public const string OperationsView = "20-1";
    public const string OperationsViewName = "Operations";
    
    public const string PremiumCalculationView = "20-9-1";
    public const string PremiumCalculationViewName = "Premium Calculation";
    
    public const string PremiumOverviewView = "20-9-10-1";
    public const string PremiumOverviewViewName = "Premium Overview View";
    
    public const string PremiumConfigurationsView = "20-9-11-1";
    public const string PremiumConfigurationsViewName = "Premium Configurations View";
    public const string PremiumConfigurationsCreate = "20-9-11-2";
    public const string PremiumConfigurationsUpdate = "20-9-11-3";
    public const string PremiumConfigurationsDelete = "20-9-11-4";
    public const string PremiumConfigurationsExport = "20-9-11-5";
    
    public const string NotificationsView = "20-14-1";
    public const string NotificationsViewName = "Notifications View";

    // System
    public const string SystemView = "21-1";
    public const string SystemViewName = "System";
    
    public const string LogsView = "21-15-1";
    public const string LogsViewName = "Logs View";
    
    public const string SystemLogView = "21-15-16-1";
    public const string SystemLogViewName = "System Log View";
    
    public const string ConfigView = "21-17-1";
    public const string ConfigViewName = "Config View";

    public const string CommonUtilitiesName = "Common Utilities";
    public const string FileUploadName = "File Upload";

    public const string BranchName = "Branch";
    public const string DesignationName = "Designation";
    public const string EntityName = "Entity";
    public const string BrandingName = "Branding";
    public const string ProfileName = "Profile";
    public const string PasswordName = "Password";
    public const string TwoFactorName = "Two Factor";
    public const string AttendanceName = "Attendance";
    public const string ReportingName = "Reporting";
    public const string MarketingExecutiveLeadsName = "My Leads";
    public const string MarketingExecutiveQuotationsName = "My Quotations";
    public const string CommonAttendanceName = "My Attendance";

    // Branch
    public const string BranchView = "18-14-1";
    public const string BranchViewName = "Branch View";
    public const string BranchCreate = "18-14-2";
    public const string BranchUpdate = "18-14-3";
    public const string BranchDelete = "18-14-4";
    public const string BranchExport = "18-14-5";

    // Designation
    public const string DesignationView = "18-15-1";
    public const string DesignationViewName = "Designation View";
    public const string DesignationCreate = "18-15-2";
    public const string DesignationUpdate = "18-15-3";
    public const string DesignationDelete = "18-15-4";
    public const string DesignationExport = "18-15-5";

    // Entity Settings (Operations)
    public const string EntitySettingsView = "20-18-1";
    public const string EntitySettingsViewName = "Entity Settings View";
    public const string EntitySettingsUpdate = "20-18-3";

    // Branding (Administration)
    public const string BrandingView = "18-16-1";
    public const string BrandingViewName = "Branding View";
    public const string BrandingUpdate = "18-16-3";

    // Profile (Administration)
    public const string ProfileView = "18-17-1";
    public const string ProfileViewName = "Profile View";
    public const string ProfileUpdate = "18-17-3";

    // Password (Administration)
    public const string PasswordChange = "18-18-3";
    public const string PasswordChangeName = "Password Change";
    public const string PasswordSet = "18-18-4";
    public const string PasswordSetName = "Password Set";

    // Two Factor (Administration)
    public const string TwoFactorView = "18-19-1";
    public const string TwoFactorViewName = "Two Factor View";
    public const string TwoFactorUpdate = "18-19-3";

    // Attendance (Operations)
    public const string AttendanceView = "20-19-1";
    public const string AttendanceViewName = "Attendance View";
    public const string AttendanceCreate = "20-19-2";
    public const string AttendanceExport = "20-19-5";

    // Marketing Executive Leads
    public const string MarketingExecutiveLeadsView = "19-14-1";
    public const string MarketingExecutiveLeadsViewName = "Marketing Executive Leads View";
    public const string MarketingExecutiveLeadsCreate = "19-14-2";
    public const string MarketingExecutiveLeadsUpdate = "19-14-3";

    // Marketing Executive Quotations
    public const string MarketingExecutiveQuotationsView = "19-15-1";
    public const string MarketingExecutiveQuotationsViewName = "Marketing Executive Quotations View";
    public const string MarketingExecutiveQuotationsCreate = "19-15-2";
    public const string MarketingExecutiveQuotationsUpdate = "19-15-3";
    public const string MarketingExecutiveQuotationsDelete = "19-15-4";

    // Common Attendance (for Marketing Executives)
    public const string CommonAttendanceView = "19-16-1";
    public const string CommonAttendanceViewName = "Common Attendance View";
    public const string CommonAttendanceCreate = "19-16-2";
    public const string CommonAttendanceCreateName = "Common Attendance Create";

    // Reporting (Operations)
    public const string ReportingView = "20-20-1";
    public const string ReportingViewName = "Reporting View";

    // Common Utilities (used by multiple roles for lookups)
    public const string CommonUtilitiesView = "22-1-1";
    public const string CommonUtilitiesViewName = "Common Utilities View";
    public const string FileUploadView = "22-2-1";
    public const string FileUploadViewName = "File Upload View";
    public const string FileUploadCreate = "22-2-2";
    public const string FileUploadCreateName = "File Upload Create";
}
public static class CmsMenuConstant
{
    public const string Todo = "Todo";
}
public enum MenuRank
{
    Dashboard = 1,
    Administration,
    SalesMarketing,
    Operations,
    System,
    // Child menus (for reference, not used in parent menus)
    Tenants,
    Roles,
    AdminManagement,
    MarketingExecutives,
    AdminLeads,
    AdminQuotations,
    PremiumCalculation,
    Notifications,
    Logs,
    Config
}
public class MenuItem
{
    public int MenuId { get; set; }
    public string MenuName { get; set; }
    public string MenuSlug { get; set; }
    public string Icon { get; set; }
    public int Rank { get; set; }
    public int Level { get; set; }

    public bool IsDisabled { get; set; } = false;
    public bool IsMenu { get; set; } = true;
    public bool ToHide { get; set; } = false;
    public bool HideChildren { get; set; }


    public List<Permission> Permissions { get; set; }
    public List<MenuItem> Children { get; set; }
}
public class Permission
{
    public string Title { get; set; }
    public string Value { get; set; }
    public bool IsAutoCheck { get; set; }

    public Permission(string title, string value, bool isAutoCheck = false)
    {
        Title = title;
        Value = value;
        IsAutoCheck = isAutoCheck;
    }
}
