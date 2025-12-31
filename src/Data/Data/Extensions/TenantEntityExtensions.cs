using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.Extensions;

/// <summary>
/// Extension methods for setting tenant-related properties on entities.
/// </summary>
public static class TenantEntityExtensions
{
    /// <summary>
    /// Sets TenantId for entities implementing ITenantEntity based on the provided tenant ID.
    /// Only sets TenantId if it's currently null or empty.
    /// </summary>
    /// <param name="changeTracker">The change tracker.</param>
    /// <param name="tenantId">The tenant identifier to set. If null or empty, no assignment occurs.</param>
    public static void SetTenantIdPropertyValues(
        this ChangeTracker changeTracker, string? tenantId)
    {
        if (string.IsNullOrEmpty(tenantId))
            return;

        // Set TenantId for new entities
        var addedTenantEntities = changeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added && string.IsNullOrEmpty(e.Entity.TenantId));

        foreach (var entry in addedTenantEntities)
        {
            entry.Entity.TenantId = tenantId;
        }

        // Set TenantId for modified entities if it's empty (for backward compatibility)
        var modifiedTenantEntities = changeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Modified && string.IsNullOrEmpty(e.Entity.TenantId));

        foreach (var entry in modifiedTenantEntities)
        {
            entry.Entity.TenantId = tenantId;
        }
    }
}


