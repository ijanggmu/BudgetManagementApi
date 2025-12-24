using System;

namespace Data.Entities.Tenant;

public enum LeadStatus { New, Qualified, Contacted, Quoted, Won, Lost }

public class Lead : TenantEntity
{
    public string ProspectId { get; set; }
    public Prospect Prospect { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public string Source { get; set; } = "Web";
    public string OwnerUserId { get; set; }
}

public class LeadActivity : TenantEntity
{
    public string LeadId { get; set; }
    public string Kind { get; set; } = "note|call|email";
    public string Notes { get; set; } = default!;
    public DateTimeOffset When { get; set; } = DateTimeOffset.UtcNow;
}


