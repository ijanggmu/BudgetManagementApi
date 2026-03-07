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
                AllowedRoles = new List<string> { "All" }, // Dashboard visible to all roles
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Administration visible to SuperAdmin and TenantAdmin
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.AdministrationViewName, MenuPermissionConstant.AdministrationView, false)
                },
                Children = new List<MenuItem>
                {
                    // 18-12. Administration > Tenants (SuperAdmin only)
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
                        AllowedRoles = new List<string> { "SuperAdmin" }, // Tenants only for SuperAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Tenants, false),
                        Children = null
                    },
                    // 18-16. Administration > Organization Details / Branding (Tenant Admin)
                    new MenuItem
                    {
                        MenuId = 16,
                        MenuName = "Organization Details",
                        MenuSlug = "/organization",
                        Icon = "BankOutlined",
                        Rank = 2,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "Admin" }, // Organization Details for TenantAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Branding, false),
                        Children = null
                    },
                    // 18-14. Administration > Branch (Tenant Admin)
                    new MenuItem
                    {
                        MenuId = 14,
                        MenuName = MenuPermissionConstant.BranchName,
                        MenuSlug = "/branch",
                        Icon = "ShopOutlined",
                        Rank = 3,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "Admin" }, // Branch for TenantAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Branch, false),
                        Children = null
                    },
                    // 18-15. Administration > Designation (Tenant Admin)
                    new MenuItem
                    {
                        MenuId = 15,
                        MenuName = MenuPermissionConstant.DesignationName,
                        MenuSlug = "/designation",
                        Icon = "IdcardOutlined",
                        Rank = 4,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "Admin" }, // Designation for TenantAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Designation, false),
                        Children = null
                    },
                    // 18-2. Administration > Roles (includes Menu permissions)
                    new MenuItem
                    {
                        MenuId = 2,
                        MenuName = MenuPermissionConstant.RolesName,
                        MenuSlug = "/roles",
                        Icon = "FileProtectOutlined",
                        Rank = 5,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Roles for SuperAdmin and TenantAdmin
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
                        Rank = 6,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin" }, // Admin Management for SuperAdmin and TenantAdmin
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin", "FoDo" }, // Sales & Marketing for all admin roles
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Marketing Executives for SuperAdmin and TenantAdmin
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Admin Leads for SuperAdmin and TenantAdmin
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Admin Quotations for SuperAdmin and TenantAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.AdminQuotations, false),
                        Children = null
                    },
                    // 19-14. Sales & Marketing > Marketing Executive Leads (for Marketing Executive role)
                    new MenuItem
                    {
                        MenuId = 14,
                        MenuName = MenuPermissionConstant.MarketingExecutiveLeadsName,
                        MenuSlug = "/leads",
                        Icon = "Target",
                        Rank = 4,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "FoDo" }, // Marketing Executive Leads for FoDo only
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.MarketingExecutiveLeads, false),
                        Children = null
                    },
                    // 19-15. Sales & Marketing > Marketing Executive Quotations (for Marketing Executive role)
                    new MenuItem
                    {
                        MenuId = 15,
                        MenuName = MenuPermissionConstant.MarketingExecutiveQuotationsName,
                        MenuSlug = "/quotations",
                        Icon = "FileText",
                        Rank = 5,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "FoDo" }, // Marketing Executive Quotations for FoDo only
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.MarketingExecutiveQuotations, false),
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin","FoDo" }, // Operations for SuperAdmin and TenantAdmin
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "FoDo" },
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
                                MenuSlug = "/premium-calculator",
                                Icon = "Calculator",
                                Rank = 1,
                                Level = 3,
                                IsDisabled = false,
                                IsMenu = true,
                                ToHide = false,
                                HideChildren = false,
                                AllowedRoles = new List<string> { "SuperAdmin", "Admin", "FoDo" },
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
                    // 20-20. Operations > Reporting
                    new MenuItem
                    {
                        MenuId = 20,
                        MenuName = MenuPermissionConstant.ReportingName,
                        MenuSlug = "/reporting",
                        Icon = "BarChartOutlined",
                        Rank = 5,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Reporting, false),
                        Children = null
                    },
                    // 20-22. Operations > Notice Board
                    new MenuItem
                    {
                        MenuId = 22,
                        MenuName = "Notice Board",
                        MenuSlug = "/noticeboard",
                        Icon = "NotificationOutlined",
                        Rank = 6,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Notice Board for SuperAdmin and TenantAdmin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.NoticeBoard, false),
                        Children = null
                    },
                    ////// 20-23. Operations > Email Gateway
                    //new MenuItem
                    //{
                    //    MenuId = 23,
                    //    MenuName = MenuPermissionConstant.EmailGatewayName,
                    //    MenuSlug = "/email-gateway",
                    //    Icon = "MailOutlined",
                    //    Rank = 7,
                    //    Level = 2,
                    //    IsDisabled = false,
                    //    IsMenu = true,
                    //    ToHide = false,
                    //    HideChildren = false,
                    //    AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // Email Gateway for SuperAdmin and TenantAdmin
                    //    Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.EmailGateway, false),
                    //    Children = null
                    //},
                    //// 20-24. Operations > SMS Gateway
                    //new MenuItem
                    //{
                    //    MenuId = 24,
                    //    MenuName = MenuPermissionConstant.SmsGatewayName,
                    //    MenuSlug = "/sms-gateway",
                    //    Icon = "MessageOutlined",
                    //    Rank = 8,
                    //    Level = 2,
                    //    IsDisabled = false,
                    //    IsMenu = true,
                    //    ToHide = false,
                    //    HideChildren = false,
                    //    AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // SMS Gateway for SuperAdmin and TenantAdmin
                    //    Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.SmsGateway, false),
                    //    Children = null
                    //},
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // System for SuperAdmin and TenantAdmin
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
            },

            // ============================================
            // 23. BUDGET MANAGEMENT (Parent Group)
            // ============================================
            new MenuItem
            {
                MenuId = 23,
                MenuName = "Budget Management",
                MenuSlug = "BudgetManagement",
                Icon = "AccountBookOutlined",
                Rank = 4,
                Level = 1,
                IsDisabled = false,
                IsMenu = true,
                ToHide = false,
                HideChildren = false,
                AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                Permissions = new List<Permission>
                {
                    new Permission("Budget Management", "23-1", false)
                },
                Children = new List<MenuItem>
                {
                    new MenuItem
                    {
                        MenuId = 1,
                        MenuName = "Budgets",
                        MenuSlug = "/budgets",
                        Icon = "DollarOutlined",
                        Rank = 1,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Budget, false),
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 3,
                        MenuName = "Memos",
                        MenuSlug = "/memos",
                        Icon = "FileTextOutlined",
                        Rank = 3,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.MemoViewName, MenuPermissionConstant.MemoView, false),
                            new Permission("Memo Create", MenuPermissionConstant.MemoCreate, false),
                            new Permission("Memo Update", MenuPermissionConstant.MemoUpdate, false),
                            new Permission("Memo Delete", MenuPermissionConstant.MemoDelete, false),
                            new Permission("Memo Export", MenuPermissionConstant.MemoExport, false),
                            new Permission("Generate PDF", MenuPermissionConstant.MemoGeneratePdf, false)
                        },
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 4,
                        MenuName = "Departments",
                        MenuSlug = "/departments",
                        Icon = "ApartmentOutlined",
                        Rank = 4,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Department, false),
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 5,
                        MenuName = "Approvals",
                        MenuSlug = "/approvals",
                        Icon = "CheckCircleOutlined",
                        Rank = 5,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = new List<Permission>
                        {
                            new Permission("Approve", MenuPermissionConstant.BudgetRequestApprove, false),
                            new Permission("Reject", MenuPermissionConstant.BudgetRequestReject, false)
                        },
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 6,
                        MenuName = "Approval Config",
                        MenuSlug = "/approval-config",
                        Icon = "SettingOutlined",
                        Rank = 6,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.ApprovalConfig, false),
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 7,
                        MenuName = "Signatures",
                        MenuSlug = "/signatures",
                        Icon = "EditOutlined",
                        Rank = 7,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = new List<Permission>
                        {
                            new Permission(MenuPermissionConstant.SignatureViewName, MenuPermissionConstant.SignatureView, false),
                            new Permission("Signature Upload", MenuPermissionConstant.SignatureUpload, false)
                        },
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 8,
                        MenuName = "Budget Reports",
                        MenuSlug = "/budget-reports",
                        Icon = "BarChartOutlined",
                        Rank = 8,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.BudgetReport, false),
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 9,
                        MenuName = "Budget Headings",
                        MenuSlug = "/budget-headings",
                        Icon = "LayersOutlined",
                        Rank = 9,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.BudgetHeadings, false),
                        Children = null
                    },
                }
            }
        });

}
