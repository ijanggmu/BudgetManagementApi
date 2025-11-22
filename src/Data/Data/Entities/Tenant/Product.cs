namespace Data.Entities.Tenant;

public class Product : TenantEntity
{
    public string Code { get; set; } = default!;     // unique per tenant
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class Coverage : TenantEntity
{
    public Guid ProductId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}


