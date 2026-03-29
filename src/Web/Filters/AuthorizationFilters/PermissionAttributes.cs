using System;
using System.Linq;
using System.Net;
using Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Constant;
using SharedKernel.Constant.Roles;
using BeemaEdgeApi.Utilities.ResponseWrapper;
using Infrastructure.Common.UserProfile;

namespace BeemaEdgeApi.Filters.AuthorizationFilters;

/// <summary>
/// Authorization filter that supports both permission-based and role-based authorization.
/// Can check permissions, roles, or both.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _permissions;
    private readonly string[] _requiredRoles;
    private readonly bool _requireBoth; // If true, user must have both role AND permission

    /// <summary>
    /// Permission-based authorization (existing behavior)
    /// </summary>
    /// <param name="permissions">Required permissions</param>
    public PermissionAttribute(params string[] permissions)
    {
        _permissions = permissions ?? Array.Empty<string>();
        _requiredRoles = Array.Empty<string>();
        _requireBoth = false;
    }

    /// <summary>
    /// Role-based authorization
    /// </summary>
    /// <param name="requiredRoles">Required roles</param>
    /// <param name="checkRoles">Set to true to enable role checking</param>
    public PermissionAttribute(string[] requiredRoles, bool checkRoles)
    {
        if (!checkRoles)
            throw new ArgumentException("checkRoles must be true when using role-based authorization", nameof(checkRoles));

        _requiredRoles = requiredRoles ?? Array.Empty<string>();
        _permissions = Array.Empty<string>();
        _requireBoth = false;
    }

    /// <summary>
    /// Combined authorization - requires both role AND permission
    /// </summary>
    /// <param name="permissions">Required permissions</param>
    /// <param name="requiredRoles">Required roles</param>
    /// <param name="requireBoth">Set to true to require both role AND permission</param>
    public PermissionAttribute(string[] permissions, string[] requiredRoles, bool requireBoth)
    {
        _permissions = permissions ?? Array.Empty<string>();
        _requiredRoles = requiredRoles ?? Array.Empty<string>();
        _requireBoth = requireBoth;
    }

    public object ClaimType { get; private set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // If no permissions or roles specified, allow access
        if ((_permissions == null || _permissions.Length == 0) &&
            (_requiredRoles == null || _requiredRoles.Length == 0))
            return;

        var roles = context.HttpContext.User.FindFirst(TokenKey.RoleId)?.Value;
        if (string.IsNullOrEmpty(roles))
        {
            SetForbiddenResult(context, "User not authenticated or no roles assigned", 4032);
            return;
        }

        var roleIds = roles.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToList();

        var roleTypeClaim = context.HttpContext.User.FindFirst(TokenKey.RoleType)?.Value;

        var hasAnyRoleRequirement = _requiredRoles is { Length: > 0 };
        var hasAnyPermissionRequirement = _permissions is { Length: > 0 };

        // Match global names (e.g. SuperAdmin) and tenant-scoped names (e.g. Admin-acme) via RoleType / prefix rules.
        var hasRequiredRole = hasAnyRoleRequirement &&
                              _requiredRoles!.Any(required =>
                                  UserMeetsRequiredRoleName(required, roleIds, roleTypeClaim));

        var hasRequiredPermission = false;
        if (hasAnyPermissionRequirement)
        {
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDataContext)) as ApplicationDataContext;
            if (dbContext == null)
            {
                SetForbiddenResult(context, "Database context not available", 4032);
                return;
            }

            var userPermissionQuery = dbContext.Roles
                                         .Join(dbContext.RoleClaims,
                                               role => role.Id,
                                               roleClaim => roleClaim.RoleId,
                                               (role, roleClaim) => new { Role = role, RoleClaim = roleClaim })
                                         .Where(rc => roleIds.Contains(rc.Role.Name) && !rc.Role.IsDeleted)
                                         .Select(rc => rc.RoleClaim.Permissions)
                                         .ToList();

            var userPermissions = userPermissionQuery.SelectMany(x => x).ToList();
            hasRequiredPermission = userPermissions.Count > 0 && userPermissions.Any(x => _permissions!.Contains(x));
        }

        bool isAuthorized;
        if (_requireBoth)
            isAuthorized = hasRequiredRole && hasRequiredPermission;
        else if (hasAnyRoleRequirement && hasAnyPermissionRequirement)
            isAuthorized = hasRequiredRole || hasRequiredPermission;
        else if (hasAnyRoleRequirement)
            isAuthorized = hasRequiredRole;
        else
            isAuthorized = hasRequiredPermission;

        if (!isAuthorized)
        {
            var message = _requireBoth
                ? $"Access denied. Required: roles ({string.Join(", ", _requiredRoles)}) AND permissions ({string.Join(", ", _permissions)})"
                : _requiredRoles.Length > 0 && _permissions.Length > 0
                    ? $"Access denied. Required: roles ({string.Join(", ", _requiredRoles)}) OR permissions ({string.Join(", ", _permissions)})"
                    : _requiredRoles.Length > 0
                        ? $"Access denied. Required roles: {string.Join(", ", _requiredRoles)}"
                        : $"Access denied. Required permissions: {string.Join(", ", _permissions)}";

            SetForbiddenResult(context, message, 4032);
        }
    }

    /// <summary>Resolves tenant-prefixed role names (Admin-tenantSlug) and RoleType claim.</summary>
    private static bool UserMeetsRequiredRoleName(string required, System.Collections.Generic.List<string> roleNamesFromToken, string? roleTypeClaim)
    {
        if (roleNamesFromToken.Any(n => string.Equals(n, required, StringComparison.OrdinalIgnoreCase)))
            return true;
        if (SystemRoles.UserRoleNamesMatch(roleNamesFromToken, required))
            return true;
        if (!string.IsNullOrEmpty(roleTypeClaim) &&
            string.Equals(roleTypeClaim, required, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    private void SetForbiddenResult(AuthorizationFilterContext context, string message, int errorCode = 4032)
    {
        var errorObject = ErrorApiResponse.WrapError(message, errorCode);
        context.Result = new ObjectResult(errorObject)
        {
            StatusCode = (int)HttpStatusCode.Forbidden,
            ContentTypes = new MediaTypeCollection { "application/json" }
        };
    }
}

/// <summary>
/// Convenience attribute for role-based authorization only
/// Usage: [RequireRoles("SuperAdmin", "Admin")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireRolesAttribute : PermissionAttribute
{
    public RequireRolesAttribute(params string[] requiredRoles)
        : base(requiredRoles, checkRoles: true)
    {
    }
}

/// <summary>
/// Requires SuperAdmin role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class SuperAdminOnlyAttribute : RequireRolesAttribute
{
    public SuperAdminOnlyAttribute()
        : base(SharedKernel.Constant.Roles.SystemRoles.SuperAdmin)
    {
    }
}

/// <summary>
/// Requires Admin or SuperAdmin role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AdminOrSuperAdminAttribute : RequireRolesAttribute
{
    public AdminOrSuperAdminAttribute()
        : base(SharedKernel.Constant.Roles.SystemRoles.Admin, SharedKernel.Constant.Roles.SystemRoles.SuperAdmin)
    {
    }
}
