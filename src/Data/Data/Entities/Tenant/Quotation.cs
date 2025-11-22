using System.Collections.Generic;

namespace Data.Entities.Tenant;

public class Quotation : TenantEntity
{
    public string Number { get; set; } = default!;    // seq per tenant
    public string Status { get; set; } = "Draft";     // Draft|Submitted|Approved|Declined|Accepted
    public string ProductId { get; set; }
    public string ProspectId { get; set; }
    public decimal? TotalPremium { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string SnapshotJson { get; set; } = "{}";  // inputs+artifacts versions
    public string? PdfUrl { get; set; }
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}

public class QuotationItem : TenantEntity
{
    public string QuotationId { get; set; }
    public string CoverageId { get; set; }
    public decimal SumInsured { get; set; }
    public decimal Premium { get; set; }
}


