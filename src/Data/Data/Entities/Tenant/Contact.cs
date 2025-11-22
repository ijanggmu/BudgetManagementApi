namespace Data.Entities.Tenant;

public class Address : TenantEntity
{
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
}

public class Contact : TenantEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; }
    public string Phone { get; set; }
    public string AddressId { get; set; }
    public Address Address { get; set; }
}

public class Prospect : TenantEntity
{
    public string PrimaryContactId { get; set; }
    public Contact PrimaryContact { get; set; }
}


