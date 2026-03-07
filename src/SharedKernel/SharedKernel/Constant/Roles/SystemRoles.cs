namespace SharedKernel.Constant.Roles;
public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string FoDo = "FoDo";
    public const string MarketingExecutive = "MarketingExecutive";

    /// <summary>Chief Executive Officer - seeded per tenant.</summary>
    public const string CEO = "CEO";
    /// <summary>Chief Financial Officer - seeded per tenant.</summary>
    public const string CFO = "CFO";
    /// <summary>Head of Department - seeded per tenant.</summary>
    public const string HOD = "HOD";

    public const int SuperAdminLevel = 999;
    public const int AdminLevel = 500;
    public const int FoDoLevel = 300;
    public const int MarketingExecutiveLevel = 300;
    public const int CEOCFOHODLevel = 200;

    public static List<string> GetDefaultAllRoles()
    {
        return new List<string>
            {
                SuperAdmin,
                Admin,
                FoDo,
                MarketingExecutive
            };
    }

    public static List<string> GetAllDefaultRolesExceptSuperAdmin()
    {
        return new List<string>
            {
                Admin,
                FoDo,
                MarketingExecutive
            };
    }

    /// <summary>Roles seeded per tenant (CEO, CFO, HOD).</summary>
    public static List<string> GetTenantDefaultRoles()
    {
        return new List<string> { CEO, CFO, HOD };
    }

    public static HashSet<string> GetNotDeletableRoles() => new HashSet<string>
            {
                SuperAdmin,
                Admin,
                FoDo,
                MarketingExecutive,
                CEO,
                CFO,
                HOD
            };
}

