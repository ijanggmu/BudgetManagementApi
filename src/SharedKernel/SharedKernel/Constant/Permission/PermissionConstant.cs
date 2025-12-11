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
