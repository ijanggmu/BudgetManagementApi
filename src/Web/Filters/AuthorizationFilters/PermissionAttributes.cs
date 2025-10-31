using System;
using System.Linq;
using System.Net;
using Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using SharedKernel.Constant;
using BeemaEdgeApi.Utilities.ResponseWrapper;

namespace BeemaEdgeApi.Filters.AuthorizationFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] permissions = null;

    public PermissionAttribute(params string[] permissions)
    {
        this.permissions = permissions;
    }

    public object ClaimType { get; private set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roles = context.HttpContext.User.FindFirst(TokenKey.RoleId)?.Value;
        var roleIds = roles?.Split(",");

        var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDataContext)) as ApplicationDataContext;

        if (permissions.Length == 0)
            return;

        var userPermissionQuery = dbContext.Roles
                                     .Join(dbContext.RoleClaims,
                                           role => role.Id,
                                           roleClaim => roleClaim.RoleId,
                                           (role, roleClaim) => new { Role = role, RoleClaim = roleClaim })
                                     .Where(rc => roleIds.Contains(rc.Role.Name))
                                     .Select(rc => rc.RoleClaim.Permissions)
                                     .ToList();

        var userPermissions = userPermissionQuery.SelectMany(x => x).ToList();

        if (userPermissions.Count > 0 && userPermissions.Any(x => permissions.Contains(x)))
            return;
        var errorObject = ErrorApiResponse.WrapError("Forbidden", 4032);

        context.Result = new ObjectResult(errorObject)
        {
            StatusCode = (int)HttpStatusCode.Forbidden,
            ContentTypes = new MediaTypeCollection { "application/json" }
        };
    }
}
