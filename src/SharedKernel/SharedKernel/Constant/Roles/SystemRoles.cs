namespace SharedKernel.Constant.Roles;

public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";

    /// <summary>Chief Executive Officer - seeded per tenant.</summary>
    public const string CEO = "CEO";
    /// <summary>Chief Financial Officer - seeded per tenant.</summary>
    public const string CFO = "CFO";
    /// <summary>Head of Department - seeded per tenant.</summary>
    public const string HOD = "HOD";
    /// <summary>Assists HOD; prepares memos for their department - seeded per tenant.</summary>
    public const string HodAssistance = "HodAssistance";

    public const int SuperAdminLevel = 999;
    public const int AdminLevel = 500;
    public const int CEOCFOHODLevel = 200;

    public static List<string> GetDefaultAllRoles()
    {
        return new List<string> { SuperAdmin, Admin };
    }

    public static List<string> GetAllDefaultRolesExceptSuperAdmin()
    {
        return new List<string> { Admin };
    }

    /// <summary>Business roles seeded per tenant (TenantAdmin is separate: Admin- plus tenant slug).</summary>
    public static List<string> GetTenantDefaultRoles()
    {
        return new List<string> { CEO, CFO, HOD, HodAssistance };
    }

    /// <summary>Identity role names that must never be deleted (global names only).</summary>
    public static HashSet<string> GetNotDeletableRoles() => new(StringComparer.OrdinalIgnoreCase)
    {
        SuperAdmin,
        Admin,
        CEO,
        CFO,
        HOD,
        HodAssistance
    };

    /// <summary>RoleType values that are system-defined and must not be edited or deleted.</summary>
    public static HashSet<string> GetNotDeletableRoleTypes() => new(StringComparer.OrdinalIgnoreCase)
    {
        SuperAdmin,
        Admin,
        CEO,
        CFO,
        HOD,
        HodAssistance
    };

    /// <summary>
    /// Matches a stored role name to a logical system role (e.g. <c>CEO-acme</c> or global <c>CEO</c>).
    /// </summary>
    public static bool RoleNameMatchesSystemRole(string? roleName, string systemRoleType)
    {
        if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(systemRoleType))
            return false;
        if (string.Equals(roleName, systemRoleType, StringComparison.OrdinalIgnoreCase))
            return true;
        var prefix = systemRoleType + "-";
        return roleName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>True if any assigned role name corresponds to the given system role type.</summary>
    public static bool UserRoleNamesMatch(IEnumerable<string>? roleNames, string systemRoleType) =>
        roleNames != null && roleNames.Any(r => RoleNameMatchesSystemRole(r, systemRoleType));
}

