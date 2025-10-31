namespace Models.Common.Policy;

public class BankDataViewModel
{
    public int SN { get; set; }
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string BankName { get; set; }
    public string BankNameInNepali { get; set; }
    public DateTime? OperationDate { get; set; }
    public string HeadOfficeLocation { get; set; }
    public decimal? PaidUpCapital { get; set; }
    public string WorkingArea { get; set; }
    public DateTime CreatedDate { get; set; }
    public string BankType { get; set; }
    public string District { get; set; }
    public string Phone1 { get; set; }
    public string Phone2 { get; set; }
    public string StreetAddress { get; set; }
}
