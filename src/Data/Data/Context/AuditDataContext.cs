using Data.Entities.Audit.UserActivites;
using Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Data.Context;
public class AuditDataContext : DbContext
{
    public AuditDataContext(DbContextOptions<AuditDataContext> options) : base(options)
    {

    }
    public DbSet<SaveChangesAudit> SaveChangesAudits { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SaveChangesAudit>(entity =>
        {
            entity.Property(p => p.Id)
            .HasValueGenerator<GuidValueGenerator>()
            .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EntityAudit>(entity =>
        {
            entity.Property(p => p.Id)
             .HasValueGenerator<GuidValueGenerator>()
             .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EntityAudit>(entity =>
        {
            entity.Property(e => e.State)
             .HasConversion(v => v.ToString(),
             v => (EntityState)Enum.Parse(typeof(EntityState), v));
        });

        modelBuilder.Entity<EntityAudit>(entity =>
        {
            entity.Property(e => e.State)
             .HasConversion(v => v.ToString(),
             v => (EntityState)Enum.Parse(typeof(EntityState), v));
        });

        base.OnModelCreating(modelBuilder);
    }
}
