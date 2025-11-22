using System;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class AttendanceEntry : TenantEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // CheckIn, CheckOut
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Remarks { get; set; }
}
