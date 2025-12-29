using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Tenancy;
using SharedKernel.Constant.Roles;
using Data.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BeemaEdgeApi.Middleware;

/// <summary>
/// Middleware for resolving tenant context with optimized caching and security validation.
/// Reduces database queries from 3-4 per request to 0-1 (cached).
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        ApplicationDataContext db,
        ITenantSchemaEnsurer schemaEnsurer,
        ITenantMigrationRunner migrationRunner,
        ITenantResolutionService tenantResolutionService,
        Infrastructure.Common.UserProfile.IUserProfileService userProfileService)
    {
        // Resolution order: explicit header -> subdomain -> userId lookup -> default
        var incoming = context.Request.Headers["X-Tenant"].FirstOrDefault();
        
        // Extract subdomain from Origin header by parsing the URL and extracting hostname
        string? subdomain = null;
        var originHeader = context.Request.Headers.Origin.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(originHeader))
        {
            try
            {
                // Parse the Origin URL (e.g., "https://hei.example.com" -> "hei.example.com")
                if (Uri.TryCreate(originHeader, UriKind.Absolute, out var originUri))
                {
                    var hostname = originUri.Host; // This removes protocol and path, gives just the hostname
                    // Only extract subdomain if it's a proper subdomain format (e.g., tenant.example.com)
                    var hostParts = hostname.Split('.');
                    if (hostParts.Length > 2)
                    {
                        subdomain = hostParts[0];
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse Origin header: {Origin}", originHeader);
            }
        }

        var slug = incoming ?? subdomain;
        
        Data.Entities.Tenant.Tenant? tenant = null;

        // First try: resolve by slug (header or subdomain) - CACHED
        if (!string.IsNullOrWhiteSpace(slug))
        {
            tenant = await tenantResolutionService.ResolveTenantBySlugAsync(slug);
        }

        // Second try: resolve by userId from authenticated user - CACHED
        if (tenant == null)
        {
            var userId = userProfileService.GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                // Get user roles and tenant ID from cache
                var userRoles = await tenantResolutionService.GetUserRolesAsync(userId);
                var userTenantId = await tenantResolutionService.GetUserTenantIdAsync(userId);

                // Check if user is SuperAdmin
                var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

                if (isSuperAdmin)
                {
                    // SuperAdmin: Explicitly set tenant context to null
                    // They can access all tenants, but we still allow explicit tenant selection via header/subdomain
                    if (!string.IsNullOrWhiteSpace(slug))
                    {
                        // SuperAdmin explicitly requested a tenant via header/subdomain
                        tenant = await tenantResolutionService.ResolveTenantBySlugAsync(slug);
                    }
                    else
                    {
                        // SuperAdmin without explicit tenant - set context to null
                        tenantContext.TenantId = null;
                        tenantContext.Slug = null;
                        await _next(context);
                        return;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(userTenantId))
                {
                    // Regular users: resolve tenant from their TenantId - CACHED
                    tenant = await tenantResolutionService.ResolveTenantByIdAsync(userTenantId);
                }
            }
        }

        // Security validation: Ensure user has access to the resolved tenant
        if (tenant != null)
        {
            var userId = userProfileService.GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userRoles = await tenantResolutionService.GetUserRolesAsync(userId);
                var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

                if (!isSuperAdmin)
                {
                    // Validate that regular users can only access their own tenant
                    var userTenantId = await tenantResolutionService.GetUserTenantIdAsync(userId);
                    if (userTenantId != tenant.Id)
                    {
                        _logger.LogWarning(
                            "User {UserId} attempted to access tenant {TenantId} but belongs to {UserTenantId}",
                            userId, tenant.Id, userTenantId);
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Access denied: You do not have access to this tenant.");
                        return;
                    }
                }
            }

            // Set tenant context
            tenantContext.TenantId = tenant.Id;
            tenantContext.Slug = tenant.Slug;

            // Ensure schema and run migrations for current tenant
            await schemaEnsurer.EnsureCurrentTenantSchemaAsync(db);
            await migrationRunner.MigrateCurrentTenantAsync();
        }

        await _next(context);
    }
}


