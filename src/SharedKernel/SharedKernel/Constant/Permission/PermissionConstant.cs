namespace SharedKernel.Constant.Permission;

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
        Create,
        Update,
        Delete,
    }


    #region Dashboard
    public const string DashboardName = "Dashboard";
    public const string DashboardViewName = "Dashboard";
    public const string DashboardView = "1-1";
    #endregion

    #region Roles
    public const string RolesName = "Roles";
    public const string RolesViewName = "Roles View";
    public const string RolesView = "18-2-1";
    #endregion

    #region Admins
    public const string AdminsName = "Admins";
    public const string AdminsViewName = "Admins View";
    public const string AdminsView = "3-1";
    #endregion

    #region Admin Management
    public const string AdminManagementName = "Admin Management";
    public const string AdminManagementViewName = "Admin Management";
    public const string AdminManagementView = "18-4-1";
    #endregion

    #region Admin Leads
    public const string AdminLeadsName = "Admin Leads";
    public const string AdminLeadsViewName = "Admin Leads View";
    public const string AdminLeadsView = "19-5-1";
    #endregion

    #region Admin Quotations
    public const string AdminQuotationsName = "Admin Quotations";
    public const string AdminQuotationsViewName = "Admin Quotations View";
    public const string AdminQuotationsView = "19-6-1";
    #endregion

    #region Leads
    public const string LeadsName = "Leads";
    public const string LeadsViewName = "Leads View";
    public const string LeadsView = "7-1";
    #endregion

    #region Quotations
    public const string QuotationsName = "Quotations";
    public const string QuotationsViewName = "Quotations View";
    public const string QuotationsView = "8-1";
    #endregion

    #region Premium Calculation
    public const string PremiumCalculationName = "Premium Calculation";
    public const string PremiumCalculationViewName = "Premium Calculation";
    public const string PremiumCalculationView = "20-9-1";

    // Premium Calculation > Overview
    public const string PremiumOverviewName = "Overview";
    public const string PremiumOverviewViewName = "Premium Overview View";
    public const string PremiumOverviewView = "20-9-10-1";

    // Premium Calculation > Configurations
    public const string PremiumConfigurationsName = "Configurations";
    public const string PremiumConfigurationsViewName = "Premium Configurations View";
    public const string PremiumConfigurationsView = "20-9-11-1";
    #endregion

    #region Tenants
    public const string TenantsName = "Tenants";
    public const string TenantsViewName = "Tenants View";
    public const string TenantsView = "18-12-1";
    #endregion

    #region Marketing Executives (formerly Fodo)
    public const string MarketingExecutivesName = "Marketing Executives";
    public const string MarketingExecutivesViewName = "Marketing Executives View";
    public const string MarketingExecutivesView = "19-13-1";
    // Keep Fodo for backward compatibility
    public const string FodoName = "Marketing Executives";
    public const string FodoViewName = "Marketing Executives View";
    public const string FodoView = "19-13-1";
    #endregion

    #region Notifications
    public const string NotificationsName = "Notifications";
    public const string NotificationsViewName = "Notifications View";
    public const string NotificationsView = "20-14-1";
    #endregion

    #region Logs
    public const string LogsName = "Logs";
    public const string LogsViewName = "Logs View";
    public const string LogsView = "21-15-1";

    // Logs > System Log
    public const string SystemLogName = "System Log";
    public const string SystemLogViewName = "System Log View";
    public const string SystemLogView = "21-15-16-1";
    #endregion

    #region Config
    public const string ConfigName = "Config";
    public const string ConfigViewName = "Config View";
    public const string ConfigView = "21-17-1";
    #endregion

    #region Administration (Parent Group)
    public const string AdministrationName = "Administration";
    public const string AdministrationViewName = "Administration";
    public const string AdministrationView = "18-1";
    #endregion

    #region Sales & Marketing (Parent Group)
    public const string SalesMarketingName = "Sales & Marketing";
    public const string SalesMarketingViewName = "Sales & Marketing";
    public const string SalesMarketingView = "19-1";
    #endregion

    #region Operations (Parent Group)
    public const string OperationsName = "Operations";
    public const string OperationsViewName = "Operations";
    public const string OperationsView = "20-1";
    #endregion

    #region System (Parent Group)
    public const string SystemName = "System";
    public const string SystemViewName = "System";
    public const string SystemView = "21-1";
    #endregion

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
