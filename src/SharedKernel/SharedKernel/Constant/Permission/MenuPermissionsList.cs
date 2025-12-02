using System.Collections.Immutable;

namespace SharedKernel.Constant.Permission;

public static partial class MenuPermissionsList
{
    public static readonly ImmutableList<MenuItem> _list = ImmutableList<MenuItem>.Empty
        .AddRange(new List<MenuItem>
        {
            // 1. Dashboard
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
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.DashboardViewName, MenuPermissionConstant.DashboardView, true)
                },
                Children = null
            },
            // 2. Roles
            new MenuItem
            {
                MenuId = 2,
                MenuName = MenuPermissionConstant.RolesName,
                MenuSlug = "/roles",
                Icon = "FileProtectOutlined",
                Rank = (int)MenuRank.Roles,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.RolesViewName, MenuPermissionConstant.RolesView, false)
                },
                Children = null
            },
            // 3. Admins
            new MenuItem
            {
                MenuId = 3,
                MenuName = MenuPermissionConstant.AdminsName,
                MenuSlug = "/admins",
                Icon = "UserOutlined",
                Rank = (int)MenuRank.Admins,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdminsViewName, MenuPermissionConstant.AdminsView, false)
                },
                Children = null
            },
            // 4. Admin Management
            new MenuItem
            {
                MenuId = 4,
                MenuName = MenuPermissionConstant.AdminManagementName,
                MenuSlug = "/admin",
                Icon = "TeamOutlined",
                Rank = (int)MenuRank.AdminManagement,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdminManagementViewName, MenuPermissionConstant.AdminManagementView, false)
                },
                Children = null
            },
            // 5. Admin Leads
            new MenuItem
            {
                MenuId = 5,
                MenuName = MenuPermissionConstant.AdminLeadsName,
                MenuSlug = "/admin-leads",
                Icon = "Target",
                Rank = (int)MenuRank.AdminLeads,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdminLeadsViewName, MenuPermissionConstant.AdminLeadsView, false)
                },
                Children = null
            },
            // 6. Admin Quotations
            new MenuItem
            {
                MenuId = 6,
                MenuName = MenuPermissionConstant.AdminQuotationsName,
                MenuSlug = "/admin-quotations",
                Icon = "FileText",
                Rank = (int)MenuRank.AdminQuotations,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdminQuotationsViewName, MenuPermissionConstant.AdminQuotationsView, false)
                },
                Children = null
            },
            // 7. Leads
            new MenuItem
            {
                MenuId = 7,
                MenuName = MenuPermissionConstant.LeadsName,
                MenuSlug = "/leads",
                Icon = "Target",
                Rank = (int)MenuRank.Leads,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.LeadsViewName, MenuPermissionConstant.LeadsView, false)
                },
                Children = null
            },
            // 8. Quotations
            new MenuItem
            {
                MenuId = 8,
                MenuName = MenuPermissionConstant.QuotationsName,
                MenuSlug = "/quotations",
                Icon = "FileText",
                Rank = (int)MenuRank.Quotations,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.QuotationsViewName, MenuPermissionConstant.QuotationsView, false)
                },
                Children = null
            },
            // 9. Premium Calculation (with children)
            new MenuItem
            {
                MenuId = 9,
                MenuName = MenuPermissionConstant.PremiumCalculationName,
                MenuSlug = "",
                Icon = "Calculator",
                Rank = (int)MenuRank.PremiumCalculation,
                Level = 1,
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
                    // 9-10. Premium Calculation > Overview
                    new MenuItem
                    {
                        MenuId = 10,
                        MenuName = MenuPermissionConstant.PremiumOverviewName,
                        MenuSlug = "/premiums",
                        Icon = "Calculator",
                        Rank = 1,
                        Level = 2,
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
                    // 9-11. Premium Calculation > Configurations
                    new MenuItem
                    {
                        MenuId = 11,
                        MenuName = MenuPermissionConstant.PremiumConfigurationsName,
                        MenuSlug = "/premiums/configurations",
                        Icon = "Settings",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.PremiumConfigurationsViewName, MenuPermissionConstant.PremiumConfigurationsView, false)
                        },
                        Children = null
                    }
                }
            },
            // 12. Tenants
            new MenuItem
            {
                MenuId = 12,
                MenuName = MenuPermissionConstant.TenantsName,
                MenuSlug = "/tenant",
                Icon = "Building2",
                Rank = (int)MenuRank.Tenants,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.TenantsViewName, MenuPermissionConstant.TenantsView, false)
                },
                Children = null
            },
            // 13. Fodo
            new MenuItem
            {
                MenuId = 13,
                MenuName = MenuPermissionConstant.FodoName,
                MenuSlug = "/fodo",
                Icon = "FileText",
                Rank = (int)MenuRank.Fodo,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.FodoViewName, MenuPermissionConstant.FodoView, false)
                },
                Children = null
            },
            // 14. Notifications
            new MenuItem
            {
                MenuId = 14,
                MenuName = MenuPermissionConstant.NotificationsName,
                MenuSlug = "/notifications",
                Icon = "BellOutlined",
                Rank = (int)MenuRank.Notifications,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.NotificationsViewName, MenuPermissionConstant.NotificationsView, false)
                },
                Children = null
            },
            // 15. Logs (with children)
            new MenuItem
            {
                MenuId = 15,
                MenuName = MenuPermissionConstant.LogsName,
                MenuSlug = "",
                Icon = "HistoryOutlined",
                Rank = (int)MenuRank.Logs,
                Level = 1,
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
                    // 15-16. Logs > System Log
                    new MenuItem
                    {
                        MenuId = 16,
                        MenuName = MenuPermissionConstant.SystemLogName,
                        MenuSlug = "/logs/system",
                        Icon = "HistoryOutlined",
                        Rank = 1,
                        Level = 2,
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
            // 17. Config
            new MenuItem
            {
                MenuId = 17,
                MenuName = MenuPermissionConstant.ConfigName,
                MenuSlug = "/config",
                Icon = "ToolOutlined",
                Rank = (int)MenuRank.Config,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.ConfigViewName, MenuPermissionConstant.ConfigView, false)
                },
                Children = null
            }
        });

}
