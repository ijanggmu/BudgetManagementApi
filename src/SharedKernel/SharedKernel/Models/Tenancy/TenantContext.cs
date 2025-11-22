using System;

namespace SharedKernel.Models.Tenancy;

public interface ITenantContext
{
    string? TenantId { get; set; }
    string? Slug { get; set; }
}

public sealed class TenantContext : ITenantContext
{
    public string? TenantId { get; set; }
    public string? Slug { get; set; }
}


