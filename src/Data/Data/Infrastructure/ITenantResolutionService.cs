using Data.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Infrastructure;

/// <summary>
/// Service for resolving tenant information with caching support.
/// </summary>
public interface ITenantResolutionService
{
    /// <summary>
    /// Resolves tenant by slug (from header or subdomain).
    /// </summary>
    Task<Tenant?> ResolveTenantBySlugAsync(string slug);

    /// <summary>
    /// Resolves tenant by tenant ID.
    /// </summary>
    Task<Tenant?> ResolveTenantByIdAsync(string tenantId);

    /// <summary>
    /// Gets user roles for a user ID.
    /// </summary>
    Task<List<string>> GetUserRolesAsync(string userId);

    /// <summary>
    /// Gets user tenant ID.
    /// </summary>
    Task<string?> GetUserTenantIdAsync(string userId);

    /// <summary>
    /// Invalidates cache for a specific tenant.
    /// </summary>
    Task InvalidateTenantCacheAsync(string tenantId, string? slug = null);

    /// <summary>
    /// Invalidates cache for a specific user.
    /// </summary>
    Task InvalidateUserCacheAsync(string userId);
}


