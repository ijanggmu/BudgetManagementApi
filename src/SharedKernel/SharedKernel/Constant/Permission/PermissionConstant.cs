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
        Operations,
        System,
        Roles,
        Admins,
        AdminManagement,
        Tenants,
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
    public const string OperationsName = "Operations";
    public const string SystemName = "System";
    public const string RolesName = "Roles";
    public const string MenuName = "Menu";
    public const string AdminManagementName = "User Management";
    public const string TenantsName = "Tenants";
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
    public const string AdminManagementViewName = "User Management";
    public const string AdminManagementCreate = "18-4-2";
    public const string AdminManagementUpdate = "18-4-3";
    public const string AdminManagementDelete = "18-4-4";
    public const string AdminManagementExport = "18-4-5";

    // Operations
    public const string OperationsView = "20-1";
    public const string OperationsViewName = "Operations";
    
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
    public const string EmailGatewayName = "Email Gateway";
    public const string SmsGatewayName = "SMS Gateway";

    public const string BranchName = "Branch";
    public const string DesignationName = "Designation";
    public const string EntityName = "Entity";
    public const string BrandingName = "Branding";
    public const string ProfileName = "Profile";
    public const string PasswordName = "Password";
    public const string TwoFactorName = "Two Factor";
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

    // Notice Board (Operations)
    public const string NoticeBoardView = "20-22-1";
    public const string NoticeBoardViewName = "Notice Board View";
    public const string NoticeBoardCreate = "20-22-2";
    public const string NoticeBoardCreateName = "Notice Board Create";
    public const string NoticeBoardUpdate = "20-22-3";
    public const string NoticeBoardUpdateName = "Notice Board Update";
    public const string NoticeBoardDelete = "20-22-4";
    public const string NoticeBoardDeleteName = "Notice Board Delete";


    // Common Utilities (used by multiple roles for lookups)
    public const string CommonUtilitiesView = "22-1-1";
    public const string CommonUtilitiesViewName = "Common Utilities View";
    public const string FileUploadView = "22-2-1";
    public const string FileUploadViewName = "File Upload View";
    public const string FileUploadCreate = "22-2-2";
    public const string FileUploadCreateName = "File Upload Create";

    // Gateway Configurations (Operations)
    public const string EmailGatewayView = "20-23-1";
    public const string EmailGatewayViewName = "Email Gateway View";
    public const string EmailGatewayCreate = "20-23-2";
    public const string EmailGatewayUpdate = "20-23-3";
    public const string EmailGatewayDelete = "20-23-4";
    public const string EmailGatewayExport = "20-23-5";

    public const string SmsGatewayView = "20-24-1";
    public const string SmsGatewayViewName = "SMS Gateway View";
    public const string SmsGatewayCreate = "20-24-2";
    public const string SmsGatewayUpdate = "20-24-3";
    public const string SmsGatewayDelete = "20-24-4";
    public const string SmsGatewayExport = "20-24-5";

    // Budget Management (23-xx)
    /// <summary>Budget Management parent menu – required to show "Budget Management" in sidebar.</summary>
    public const string BudgetManagementView = "23-1";
    public const string BudgetView = "23-1-1";
    public const string BudgetViewName = "Budget View";
    public const string BudgetCreate = "23-1-2";
    public const string BudgetUpdate = "23-1-3";
    public const string BudgetDelete = "23-1-4";
    public const string BudgetExport = "23-1-5";
    public const string BudgetRequestView = "23-2-1";
    public const string BudgetRequestViewName = "Budget Request View";
    public const string BudgetRequestCreate = "23-2-2";
    public const string BudgetRequestApprove = "23-2-3";
    public const string BudgetRequestReject = "23-2-4";
    public const string MemoView = "23-3-1";
    public const string MemoViewName = "Memo View";
    public const string MemoCreate = "23-3-2";
    public const string MemoUpdate = "23-3-3";
    public const string MemoDelete = "23-3-4";
    public const string MemoExport = "23-3-5";
    public const string MemoGeneratePdf = "23-3-6";
    public const string DepartmentView = "23-4-1";
    public const string DepartmentViewName = "Department View";
    public const string DepartmentCreate = "23-4-2";
    public const string DepartmentUpdate = "23-4-3";
    public const string DepartmentDelete = "23-4-4";
    public const string ApprovalConfigView = "23-5-1";
    public const string ApprovalConfigViewName = "Approval Config View";
    public const string ApprovalConfigCreate = "23-5-2";
    public const string ApprovalConfigUpdate = "23-5-3";
    public const string ApprovalConfigDelete = "23-5-4";
    public const string SignatureView = "23-6-1";
    public const string SignatureViewName = "Signature View";
    public const string SignatureUpload = "23-6-2";
    public const string BudgetReportView = "23-7-1";
    public const string BudgetReportViewName = "Budget Report View";
    public const string BudgetReportExport = "23-7-5";
    public const string BudgetHeadingsView = "23-9-1";
    public const string BudgetHeadingsViewName = "Budget Headings View";
    public const string BudgetHeadingsCreate = "23-9-2";
    public const string BudgetHeadingsUpdate = "23-9-3";
    public const string BudgetHeadingsDelete = "23-9-4";
    public const string BudgetHeadingsExport = "23-9-5";
    public const string NepaliFiscalYearView = "23-10-1";
    public const string NepaliFiscalYearViewName = "Fiscal Year View";
    public const string NepaliFiscalYearCreate = "23-10-2";
    public const string NepaliFiscalYearUpdate = "23-10-3";
    public const string NepaliFiscalYearDelete = "23-10-4";
    public const string NepaliFiscalYearExport = "23-10-5";
}
public static class CmsMenuConstant
{
    public const string Todo = "Todo";
}
public enum MenuRank
{
    Dashboard = 1,
    Administration,
    Operations,
    System,
    // Child menus (for reference, not used in parent menus)
    Tenants,
    Roles,
    AdminManagement,
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

    /// <summary>
    /// List of roles that can see this menu item. 
    /// If null or empty, menu is visible to all roles.
    /// Values: "SuperAdmin", "Admin", or "All"
    /// </summary>
    public List<string>? AllowedRoles { get; set; }

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
