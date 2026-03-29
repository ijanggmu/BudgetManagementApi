using System;
using System.Linq;
using SharedKernel.Constant.Roles;

namespace Infrastructure.Common.UserProfile;

/// <summary>
/// SuperAdmin detection using <see cref="TokenKey.RoleType"/> (preferred) and role name claims.
/// Do not use substring checks on role name lists (e.g. "Admin-ejan" must not match "SuperAdmin").
/// </summary>
public static class SuperAdminExtensions
{
    public static bool IsSuperAdmin(this IUserProfileService profile)
    {
        var roleType = profile.GetRoleType();
        if (string.Equals(roleType, SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var name in SplitRoleNames(profile.GetRoleId()))
        {
            if (string.Equals(name, SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static string[] SplitRoleNames(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Array.Empty<string>();
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
