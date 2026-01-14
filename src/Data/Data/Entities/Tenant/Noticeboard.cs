using System;

namespace Data.Entities.Tenant;

/// <summary>
/// Noticeboard entity for tenant-specific announcements and notices
/// </summary>
public class Noticeboard : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPinned { get; set; } = false; // Pinned notices appear at the top
    public int Priority { get; set; } = 0; // Higher priority notices appear first
}
