using System.Threading.Tasks;
using Data.Entities.Tenant;

namespace Business.Common.TenantDomain;

public interface IBrandingService
{
    Task<CompanyBranding?> GetAsync();
    Task<CompanyBranding> UpdateAsync(string? logoUrl, string? paletteJson, string? typographyJson);
}


