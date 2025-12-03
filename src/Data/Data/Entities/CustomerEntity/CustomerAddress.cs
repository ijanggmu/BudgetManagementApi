using Data.Entities.BaseEntity;
namespace Data.Entities.CustomerEntity;

public class CustomerAddress : ApplicationBaseEntity
{
    public string Province { get; set; }
    public string District { get; set; }
    public string Municipality { get; set; }
    public int? Ward { get; set; }
    public string StreetAddress { get; set; }
    public string AddressType { get; set; }
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}
