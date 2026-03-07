using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class MemoTemplateConfiguration : IEntityTypeConfiguration<MemoTemplate>
{
    public void Configure(EntityTypeBuilder<MemoTemplate> builder)
    {
        builder.ToTable("MemoTemplates");
        builder.HasIndex(x => new { x.Name, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(MemoTemplateConfiguration))]
public class MemoTemplate : TenantEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    /// <summary>
    /// Optional HTML or plain text body template (placeholders like {{Purpose}}, {{Amount}}).
    /// </summary>
    public string? BodyTemplate { get; set; }
}
