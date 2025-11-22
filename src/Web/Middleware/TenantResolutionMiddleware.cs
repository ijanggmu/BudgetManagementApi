using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Tenancy;
using SharedKernel.Constant;
using SharedKernel.Constant.Roles;
using Data.Infrastructure;

namespace BeemaEdgeApi.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        ApplicationDataContext db,
        ITenantSchemaEnsurer schemaEnsurer,
        ITenantMigrationRunner migrationRunner,
        Infrastructure.Common.UserProfile.IUserProfileService userProfileService)
    {
        // Resolution order: explicit header -> subdomain -> userId lookup -> default
        var incoming = context.Request.Headers["X-Tenant"].FirstOrDefault();
        var host = context.Request.Host.Host;
        var subdomain = host.Split('.').Length > 2 ? host.Split('.')[0] : null;

        var slug = incoming ?? subdomain;
        Data.Entities.Tenant.Tenant tenant = null;

        // First try: resolve by slug (header or subdomain)
        if (!string.IsNullOrWhiteSpace(slug))
        {
            tenant = await db.Set<Data.Entities.Tenant.Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);
        }

        // Second try: resolve by userId from JWT token/claims
        if (tenant == null)
        {
            var username = userProfileService.GetUsername();
            if (!string.IsNullOrWhiteSpace(username))
            {
                var user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted && !u.IsDisabled);

                if (user != null)
                {
                    // Check if user is SuperAdmin - SuperAdmin can access all tenants
                    var userRoles = await db.UserRoles
                        .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                        .Where(x => x.UserId == username)
                        .Select(x => x.Name)
                        .ToListAsync();

                    // If SuperAdmin, don't restrict to a specific tenant (can access all)
                    if (userRoles.Contains(SystemRoles.SuperAdmin))
                    {
                        // SuperAdmin doesn't need tenant resolution - can work across all tenants
                        // Tenant will be resolved from header/subdomain if needed, but not required
                    }
                    else if (!string.IsNullOrWhiteSpace(user.TenantId))
                    {
                        // Regular users: resolve tenant from their TenantId
                        tenant = await db.Set<Data.Entities.Tenant.Tenant>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(t => t.Id == user.TenantId && t.IsActive);
                    }
                }
            }
        }

        if (tenant is not null)
        {
            tenantContext.TenantId = tenant.Id;
            tenantContext.Slug = tenant.Slug;
            // ensure schema and run migrations for current tenant
            await schemaEnsurer.EnsureCurrentTenantSchemaAsync(db);
            await migrationRunner.MigrateCurrentTenantAsync();
        }

        await _next(context);
    }
}


