using Data.Entities.BaseEntity;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.FodoEntity;
public class FodoConfiguration : IEntityTypeConfiguration<Fodo>
{
    public void Configure(EntityTypeBuilder<Fodo> builder)
    {
        builder.ToTable("Fodos");
        builder.HasIndex(x => x.FullName).IsUnique(false);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        
        // Relationships
        builder.HasOne(x => x.Designation)
            .WithMany()
            .HasForeignKey(x => x.DesignationId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

[EntityTypeConfiguration(typeof(FodoConfiguration))]
public class Fodo : ApplicationBaseEntity, ITenantEntity
{
    public string FullName { get; set; }
    public string EmployeeId { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    // Designation and Branch references
    public string? DesignationId { get; set; }
    public virtual Designation? Designation { get; set; }
    public string? BranchId { get; set; }
    public virtual Branch? Branch { get; set; }
    
    // Permanent Address
    public string? PermanentProvince { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? PermanentMunicipality { get; set; }
    public int? PermanentWard { get; set; }
    
    // Temporary Address
    public string? TemporaryProvince { get; set; }
    public string? TemporaryDistrict { get; set; }
    public string? TemporaryMunicipality { get; set; }
    public int? TemporaryWard { get; set; }
    
    // Status: Active/Inactive
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
}
