using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Tenancy;

namespace Business.Common.TenantDomain;

public class BrandingService : IBrandingService
{
    private readonly ApplicationDataContext _db;
    private readonly ITenantContext _tenant;

    public BrandingService(ApplicationDataContext db, ITenantContext tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<CompanyBranding?> GetAsync()
    {
        if (_tenant.TenantId is null) return null;
        return await _db.Set<CompanyBranding>().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == _tenant.TenantId);
    }

    public async Task<CompanyBranding> UpdateAsync(string? logoUrl, string? paletteJson, string? typographyJson)
    {
        if (_tenant.TenantId is null) throw new InvalidOperationException("Tenant not resolved");
        var branding = await _db.Set<CompanyBranding>().FirstOrDefaultAsync(x => x.TenantId == _tenant.TenantId);
        if (branding is null)
        {
            branding = new CompanyBranding { TenantId = _tenant.TenantId };
            _db.Add(branding);
        }
        if (!string.IsNullOrWhiteSpace(logoUrl)) branding.LogoUrl = logoUrl;
        if (!string.IsNullOrWhiteSpace(paletteJson)) branding.PaletteJson = paletteJson;
        if (!string.IsNullOrWhiteSpace(typographyJson)) branding.TypographyJson = typographyJson;
        branding.Version += 1;
        await _db.SaveChangesAsync();
        return branding;
    }
}


