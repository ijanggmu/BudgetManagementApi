using System.Collections.Generic;
using Models.Common.Policy.Policy;

namespace Data.Entities.Tenant;

public class Quotation : TenantEntity
{
    public string Number { get; set; } = default!;    // seq per tenant
    public string Status { get; set; } = "Draft";     // Draft|Submitted|Approved|Declined|Accepted
    public string ProductId { get; set; }
    public string LeadId { get; set; }
    public Lead Lead { get; set; }
    public InsuranceType InsuranceType { get; set; }
    public string PortfolioAlias { get; set; }
    public string ProspectId { get; set; }
    public decimal? TotalPremium { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public bool? IsDirectBusiness { get; set; } // Broker/Agent: Yes/No
    public string SnapshotJson { get; set; } = "{}";  // inputs+artifacts versions
    public string? PdfUrl { get; set; }
    public string? PremiumCalculationJson { get; set; }
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}

public class QuotationItem : TenantEntity
{
    public string QuotationId { get; set; }
    public string CoverageId { get; set; }
    public decimal SumInsured { get; set; }
    public decimal Premium { get; set; }
}



//public class Quotation : TenantEntity
//{
//    public string Number { get; set; } = default!;    // seq per tenant
//    public string Status { get; set; } = "Draft";     // Draft|Submitted|Approved|Declined|Accepted
//    // Insurance Category & Type
//    //public InsuranceCategory InsuranceCategory { get; set; }
//    //public InsurancePackage Package { get; set; } // Comprehensive, Third Party
//    public InsuranceType InsuranceType { get; set; }

//    public string ProductId { get; set; }
//    public string ProspectId { get; set; }
//    // Discounts & Options
//    public int? ExcessOnOwnDamage { get; set; } // 500, 1000, 2000
//    public bool? IsDirectBusiness { get; set; } // Broker/Agent: Yes/No
//    public bool? CoversRiotOrTerrorism { get; set; }
//    public int NoClaimDiscountYears { get; set; } // 0, 1, 2, 3+ years
//    public decimal ClaimDiscountPercentage { get; set; } // 0%, 15%, 25%, 35%
//    public decimal? TotalPremium { get; set; }
//    public decimal? DiscountPercent { get; set; }
//    public DateOnly? ValidUntil { get; set; }
//    // Additional Info
//    public string SnapshotJson { get; set; } = "{}";  // inputs+artifacts versions
//    public string? PdfUrl { get; set; }
//    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
//}

//public class QuotationItem : TenantEntity
//{
//    public string QuotationId { get; set; }
//    public string CoverageId { get; set; }
//    public int? EngineCapacity { get; set; } // in CC (0-149cc)
//    public decimal? KiloWattRange { get; set; } // for Electric vehicles
//    //public string? ManufactureYear { get; set; }
//    //public DateTime? DateOfPurchase { get; set; }
//    public DateTime? YearOfRegistration { get; set; }
//    public decimal MarketValue { get; set; }
//    public decimal SumInsured { get; set; }
//    public decimal Premium { get; set; }
//    public bool IsTaxable { get; set; }
//    public decimal TaxRate { get; set; }
//    public Quotation Quotation { get; set; } = null!;
//}
