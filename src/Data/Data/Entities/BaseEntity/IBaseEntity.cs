namespace Data.Entities.BaseEntity;

/// <summary>
/// Base interface for all entities in the system.
/// Provides soft delete capability through IsDeleted property.
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Indicates whether the entity has been soft deleted.
    /// When true, the entity is excluded from queries by default.
    /// </summary>
    bool IsDeleted { get; set; }
}
