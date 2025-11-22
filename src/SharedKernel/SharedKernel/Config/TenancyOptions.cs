namespace SharedKernel.Config;

public enum TenantStorageStrategy
{
    SharedTable = 0,
    SeparateSchema = 1,
    SeparateDatabase = 2
}

public class TenancyOptions
{
    public TenantStorageStrategy Strategy { get; set; } = TenantStorageStrategy.SharedTable;
    public string? SchemaPrefix { get; set; } = "tenant_"; // used for SeparateSchema
}


