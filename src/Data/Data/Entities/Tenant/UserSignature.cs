using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class UserSignatureConfiguration : IEntityTypeConfiguration<UserSignature>
{
    public void Configure(EntityTypeBuilder<UserSignature> builder)
    {
        builder.ToTable("UserSignatures");
        builder.HasIndex(x => new { x.UserId, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(UserSignatureConfiguration))]
public class UserSignature : TenantEntity
{
    public string UserId { get; set; } = default!;
    public string SignatureUrl { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
}
