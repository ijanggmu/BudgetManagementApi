using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Tenancy;
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

        // Second try: resolve by userId from authenticated user (authentication has already run)
        if (tenant == null)
        {
            // Authentication middleware has already run, so we can use the validated claims
            var userId = userProfileService.GetUserId();
            var username = userProfileService.GetUsername();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsDisabled);

                if (user != null)
                {
                    // Check if user is SuperAdmin - SuperAdmin can access all tenants
                    var userRoles = await db.UserRoles
                        .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                        .Join(db.Roles
                            .Where(r => !r.IsDeleted),
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
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


