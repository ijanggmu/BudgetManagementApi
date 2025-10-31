using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.Extensions;


/// <summary>
/// Extension methods for adding auditable shadow properties to Entity Framework Core entities.
/// </summary>
public static class AuditableShadowProperties
{
    public static readonly Func<object, DateTime?> EfPropertyCreatedDateTime =
       entity => EF.Property<DateTime?>(entity, CreatedOn);

    public static readonly string CreatedOn = nameof(CreatedOn);

    public static readonly Func<object, DateTime?> EfPropertyModifiedDateTime =
        entity => EF.Property<DateTime?>(entity, LastModifiedOn);

    public static readonly string LastModifiedOn = nameof(LastModifiedOn);

    public static readonly Func<object, string> EfPropertyCreatedBy =
        entity => EF.Property<string>(entity, CreatedBy);

    public static readonly string CreatedBy = nameof(CreatedBy);

    public static readonly Func<object, string> EfPropertyLastModifiedBy =
        entity => EF.Property<string>(entity, LastModifiedBy);

    public static readonly string LastModifiedBy = nameof(LastModifiedBy);

    private const int MaxUserNameLength = 128;

    /// <summary>
    /// Adds auditable shadow properties to the specified model builder.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void AddAuditableShadowProperties(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model
                                               .GetEntityTypes()
                                               .Where(e => typeof(IBaseEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property<DateTime?>(CreatedOn);

            modelBuilder.Entity(entityType.ClrType)
                .Property<DateTime?>(LastModifiedOn);

            modelBuilder.Entity(entityType.ClrType)
                .Property<string>(CreatedBy)
                .HasMaxLength(MaxUserNameLength);

            modelBuilder.Entity(entityType.ClrType)
                .Property<string>(LastModifiedBy)
                .HasMaxLength(MaxUserNameLength);
        }
    }

    /// <summary>
    /// Sets auditable entity property values based on the change tracker.
    /// </summary>
    /// <param name="changeTracker">The change tracker.</param>
    /// <param name="userId">The user identifier.</param>
    public static void SetAuditableEntityPropertyValues(
        this ChangeTracker changeTracker, string userId = default, DateTime currentTime = default)
    {
        currentTime = currentTime == default ? DateTime.UtcNow : currentTime;

        var modifiedEntries = changeTracker.Entries<IBaseEntity>()
                                           .Where(x => x.State == EntityState.Modified);

        foreach (var modifiedEntry in modifiedEntries)
        {
            modifiedEntry.Property(LastModifiedOn).CurrentValue = currentTime;
            if (!string.IsNullOrEmpty(userId))
                modifiedEntry.Property(LastModifiedBy).CurrentValue = userId;
        }

        var addedEntries = changeTracker.Entries<IBaseEntity>()
            .Where(x => x.State == EntityState.Added);

        foreach (var addedEntry in addedEntries)
        {
            addedEntry.Property(CreatedOn).CurrentValue = currentTime;

            if (!string.IsNullOrEmpty(userId))
                addedEntry.Property(CreatedBy).CurrentValue = userId;
        }
    }
}
