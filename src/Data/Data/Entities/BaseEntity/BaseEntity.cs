using System.ComponentModel.DataAnnotations;

namespace Data.Entities.BaseEntity;

/// <summary>
/// Base entity with string Primary Key. 
/// Use this entity when table with string Primary Key.
/// </summary>
public class ApplicationBaseEntity : IBaseEntity, IAuditableEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
}


/// <summary>
/// Base entity with int Primary Key. 
/// Use this entity when table with int Primary Key is needed.
/// </summary>
public class ApplicationIntBaseEntity : IBaseEntity, IAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
}
