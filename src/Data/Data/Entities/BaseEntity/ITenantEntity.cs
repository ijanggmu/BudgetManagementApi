namespace Data.Entities.BaseEntity;

/// <summary>
/// Interface for entities that have a TenantId property.
/// Used to automatically set TenantId when saving entities.
/// </summary>
public interface ITenantEntity
{
    string TenantId { get; set; }
}

