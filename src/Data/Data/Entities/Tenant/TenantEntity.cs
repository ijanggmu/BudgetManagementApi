using System;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public abstract class TenantEntity : ApplicationBaseEntity, ITenantEntity
{
    public string TenantId { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}


