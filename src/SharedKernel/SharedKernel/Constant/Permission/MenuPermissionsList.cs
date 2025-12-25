using System.Collections.Immutable;
using System.Linq;

namespace SharedKernel.Constant.Permission;

public static partial class MenuPermissionsList
{
    public static readonly ImmutableList<MenuItem> _list = ImmutableList<MenuItem>.Empty
        .AddRange(new List<MenuItem>
        {
            // ============================================
            // 1. DASHBOARD (Standalone)
            // ============================================
            new MenuItem
            {
                MenuId = 1,
                MenuName = MenuPermissionConstant.DashboardName,
                MenuSlug = "/",
                Icon = "HomeOutlined",
                Rank = (int)MenuRank.Dashboard,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions =
                [
                    new Permission(MenuPermissionConstant.DashboardViewName, MenuPermissionConstant.DashboardView, true),
                    .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.TwoFactor, false),
                    .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Password, false),
                    .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.FileUpload, false),
                    .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.CommonUtilities, false),
                    .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Profile, false),

                ],
                Children = null
            },

            // ============================================
            // 18. ADMINISTRATION (Parent Group)
            // ============================================
            new MenuItem
            {
                MenuId = 18,
                MenuName = MenuPermissionConstant.AdministrationName,
                MenuSlug = "Adminstration",
                Icon = "SettingOutlined",
                Rank = (int)MenuRank.Administration,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdministrationViewName, MenuPermissionConstant.AdministrationView, false)
                },
                Children = new List<MenuItem>
                {
                    // 18-12. Administration > Tenants
                    new MenuItem
                    {
                        MenuId = 12,
                        MenuName = MenuPermissionConstant.TenantsName,
                        MenuSlug = "/tenant",
                        Icon = "Building2",
                        Rank = 1,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions =
                        [
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Tenants, false),
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Branch, false),
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Designation, false),
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Branding, false),
                        ],
                        Children = null
                    },
                    // 18-2. Administration > Roles (includes Menu permissions)
                    new MenuItem
                    {
                        MenuId = 2,
                        MenuName = MenuPermissionConstant.RolesName,
                        MenuSlug = "/roles",
                        Icon = "FileProtectOutlined",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions =
                        [
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Roles, false),
                            .. PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Menu, false),
                        ],
                        Children = null
                    },
                    // 18-4. Administration > Admin Management
                    new MenuItem
                    {
                        MenuId = 4,
                        MenuName = MenuPermissionConstant.AdminManagementName,
                        MenuSlug = "/admin",
                        Icon = "TeamOutlined",
                        Rank = 3,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.AdminManagement, false),
                        Children = null
                    },

                }
            },

            // ============================================
            // 19. SALES & MARKETING (Parent Group)
            // ============================================
            new MenuItem
            {
                MenuId = 19,
                MenuName = MenuPermissionConstant.SalesMarketingName,
                MenuSlug = "SalesMarketing",
                Icon = "ShoppingOutlined",
                Rank = (int)MenuRank.SalesMarketing,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.SalesMarketingViewName, MenuPermissionConstant.SalesMarketingView, false)
                },
                Children = new List<MenuItem>
                {
                    // 19-13. Sales & Marketing > Marketing Executives
                    new MenuItem
                    {
                        MenuId = 13,
                        MenuName = MenuPermissionConstant.MarketingExecutivesName,
                        MenuSlug = "/fodo",
                        Icon = "UserOutlined",
                        Rank = 1,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.MarketingExecutives, false),
                        Children = null
                    },
                    // 19-5. Sales & Marketing > Admin Leads
                    new MenuItem
                    {
                        MenuId = 5,
                        MenuName = MenuPermissionConstant.AdminLeadsName,
                        MenuSlug = "/admin-leads",
                        Icon = "Target",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.AdminLeads, false),
                        Children = null
                    },
                    // 19-6. Sales & Marketing > Admin Quotations
                    new MenuItem
                    {
                        MenuId = 6,
                        MenuName = MenuPermissionConstant.AdminQuotationsName,
                        MenuSlug = "/admin-quotations",
                        Icon = "FileText",
                        Rank = 3,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.AdminQuotations, false),
                        Children = null
                    },
                }
            },

            // ============================================
            // 20. OPERATIONS (Parent Group)
            // ============================================
            new MenuItem
            {
                MenuId = 20,
                MenuName = MenuPermissionConstant.OperationsName,
                MenuSlug = "Operation",
                Icon = "ToolOutlined",
                Rank = (int)MenuRank.Operations,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.OperationsViewName, MenuPermissionConstant.OperationsView, false)
                },
                Children = new List<MenuItem>
                {
                    // 20-9. Operations > Premium Calculation (with children)
                    new MenuItem
                    {
                        MenuId = 9,
                        MenuName = MenuPermissionConstant.PremiumCalculationName,
                        MenuSlug = "Permium",
                        Icon = "Calculator",
                        Rank = 1,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.PremiumCalculationViewName, MenuPermissionConstant.PremiumCalculationView, false)
                        },
                        Children = new List<MenuItem>
                        {
                            // 20-9-10. Operations > Premium Calculation > Overview
                            new MenuItem
                            {
                                MenuId = 10,
                                MenuName = MenuPermissionConstant.PremiumOverviewName,
                                MenuSlug = "/premiums",
                                Icon = "Calculator",
                                Rank = 1,
                                Level = 3,
                                IsDisabled = false,
                                IsMenu = true,
                                ToHide = false,
                                HideChildren = false,
                                Permissions = new List<Permission>
                                {
                                    new Permission(MenuPermissionConstant.PremiumOverviewViewName, MenuPermissionConstant.PremiumOverviewView, false)
                                },
                                Children = null
                            },
                            // 20-9-11. Operations > Premium Calculation > Configurations
                            new MenuItem
                            {
                                MenuId = 11,
                                MenuName = MenuPermissionConstant.PremiumConfigurationsName,
                                MenuSlug = "/premiums/configurations",
                                Icon = "Settings",
                                Rank = 2,
                                Level = 3,
                                IsDisabled = false,
                                IsMenu = true,
                                ToHide = false,
                                HideChildren = false,
                                Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.PremiumConfigurations, false),
                                Children = null
                            }
                        }
                    },
                    // 20-14. Operations > Notifications
                    new MenuItem
                    {
                        MenuId = 14,
                        MenuName = MenuPermissionConstant.NotificationsName,
                        MenuSlug = "/notifications",
                        Icon = "BellOutlined",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions =
                        [
                            new Permission(MenuPermissionConstant.NotificationsViewName, MenuPermissionConstant.NotificationsView, false)
                        ],
                        Children = null
                    },
                    // 20-18. Operations > Entity Settings
                    new MenuItem
                    {
                        MenuId = 18,
                        MenuName = MenuPermissionConstant.EntityName,
                        MenuSlug = "/entity",
                        Icon = "SettingOutlined",
                        Rank = 3,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Entity, false),
                        Children = null
                    },
                    // 20-35. Operations > Attendance
                    new MenuItem
                    {
                        MenuId = 35,
                        MenuName = MenuPermissionConstant.AttendanceName,
                        MenuSlug = "/attendance",
                        Icon = "CalendarOutlined",
                        Rank = 4,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Attendance, false),
                        Children = null
                    },
                }
            },

            // ============================================
            // 21. SYSTEM (Parent Group)
            // ============================================
            new MenuItem
            {
                MenuId = 21,
                MenuName = MenuPermissionConstant.SystemName,
                MenuSlug = "System",
                Icon = "DatabaseOutlined",
                Rank = (int)MenuRank.System,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.SystemViewName, MenuPermissionConstant.SystemView, false)
                },
                Children = new List<MenuItem>
                {
                    // 21-15. System > Logs (with children)
                    new MenuItem
                    {
                        MenuId = 15,
                        MenuName = MenuPermissionConstant.LogsName,
                        MenuSlug = "logs",
                        Icon = "HistoryOutlined",
                        Rank = 1,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.LogsViewName, MenuPermissionConstant.LogsView, false)
                        },
                        Children = new List<MenuItem>
                        {
                            // 21-15-16. System > Logs > System Log
                            new MenuItem
                            {
                                MenuId = 16,
                                MenuName = MenuPermissionConstant.SystemLogName,
                                MenuSlug = "/logs/system",
                                Icon = "HistoryOutlined",
                                Rank = 1,
                                Level = 3,
                                IsDisabled = false,
                                IsMenu = true,
                                ToHide = false,
                                HideChildren = false,
                                Permissions = new List<Permission>
                                {
                                    new Permission(MenuPermissionConstant.SystemLogViewName, MenuPermissionConstant.SystemLogView, false)
                                },
                                Children = null
                            }
                        }
                    },
                    // 21-17. System > Config
                    new MenuItem
                    {
                        MenuId = 17,
                        MenuName = MenuPermissionConstant.ConfigName,
                        MenuSlug = "/config",
                        Icon = "ToolOutlined",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.ConfigViewName, MenuPermissionConstant.ConfigView, false)
                        },
                        Children = null
                    },

                }
            }
        });

}
