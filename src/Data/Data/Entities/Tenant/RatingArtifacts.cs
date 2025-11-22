namespace Data.Entities.Tenant;

public class RateFactor : TenantEntity
{
    public Guid ProductId { get; set; }
    public string Key { get; set; } = default!;      // "age","region"
    public string DataType { get; set; } = "number"; // number|text|enum|bool
    public string AllowedValuesJson { get; set; }   // for enum; ["A1","A2"]
}

public class RateTable : TenantEntity
{
    public Guid ProductId { get; set; }
    public int Version { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string SchemaJson { get; set; } = "{}";   // columns + types
    public string SourceUri { get; set; } = default!; // blob path to CSV/JSON
}

public class PremiumFormula : TenantEntity
{
    public Guid ProductId { get; set; }
    public int Version { get; set; }
    public string Expression { get; set; } = default!; // "Base*AgeFactor*Region + Fees - Discount"
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}

public class UnderwritingRule : TenantEntity
{
    public Guid ProductId { get; set; }
    public string Expression { get; set; } = default!; // bool expression
    public string Message { get; set; } = "Not eligible";
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}


