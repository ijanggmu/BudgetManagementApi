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
                    // 18-4. Administration > User Management
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin" }, // User Management for SuperAdmin and Tenant Admin
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.AdminManagement, false),
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
                Permissions = new List<Permission>
                {
                    new Permission(MenuPermissionConstant.OperationsViewName, MenuPermissionConstant.OperationsView, false)
                },
                Children = new List<MenuItem>
                {
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
                    // 20-18. Operations > Entity Settings — hidden from menu
                    //new MenuItem
                    //{
                    //    MenuId = 18,
                    //    MenuName = MenuPermissionConstant.EntityName,
                    //    MenuSlug = "/entity",
                    //    Icon = "SettingOutlined",
                    //    Rank = 3,
                    //    Level = 2,
                    //    IsDisabled = false,
                    //    IsMenu = true,
                    //    ToHide = false,
                    //    HideChildren = false,
                    //    Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.Entity, false),
                    //    Children = null
                    //},
                    // 20-22. Operations > Notice Board — hidden from menu
                    //new MenuItem
                    //{
                    //    MenuId = 22,
                    //    MenuName = "Notice Board",
                    //    MenuSlug = "/noticeboard",
                    //    Icon = "NotificationOutlined",
                    //    Rank = 6,
                    //    Level = 2,
                    //    IsDisabled = false,
                    //    IsMenu = true,
                    //    ToHide = false,
                    //    HideChildren = false,
                    //    AllowedRoles = new List<string> { "SuperAdmin", "Admin" },
                    //    Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.NoticeBoard, false),
                    //    Children = null
                    //},
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
                AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
                Permissions = new List<Permission>
                {
                    new Permission("Budget Management", MenuPermissionConstant.BudgetManagementView, false)
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
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
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.BudgetHeadings, false),
                        Children = null
                    },
                    new MenuItem
                    {
                        MenuId = 10,
                        MenuName = "Fiscal Years",
                        MenuSlug = "/nepali-fiscal-years",
                        Icon = "CalendarOutlined",
                        Rank = 10,
                        Level = 2,
                        IsDisabled = false,
                        IsMenu = true,
                        ToHide = false,
                        HideChildren = false,
                        AllowedRoles = new List<string> { "SuperAdmin", "Admin", "CEO", "CFO", "HOD", "HodAssistance" },
                        Permissions = PermissionHelper.ToPermissionList(MenuPermissionDefinitions.NepaliFiscalYear, false),
                        Children = null
                    },
                }
            }
        });

}
