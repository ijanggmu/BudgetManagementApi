using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Entity;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class EntitySettingsService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService)
    : IEntitySettingsService
{
    public async Task<Result<EntitySettingsResponseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<EntitySettingsResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<EntitySettingsResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<EntitySettingsResponseDto>.Failed("Unauthorized access.");

        var tenantId = isSuperAdmin ? user.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId))
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null)
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var branding = await db.CompanyBrandings
            .FirstOrDefaultAsync(b => b.TenantId == tenantId, cancellationToken);

        if (tenant == null)
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        // Extract primary color from PaletteJson (simplified - you may need to parse JSON)
        var primaryColor = "#000000"; // Default
        if (branding != null && !string.IsNullOrEmpty(branding.PaletteJson))
        {
            // Simple extraction - in production, parse JSON properly
            if (branding.PaletteJson.Contains("\"primary\""))
            {
                // Extract primary color from JSON
                // This is a simplified version - you should use proper JSON parsing
                var primaryIndex = branding.PaletteJson.IndexOf("\"primary\"");
                if (primaryIndex > 0)
                {
                    var colorStart = branding.PaletteJson.IndexOf("#", primaryIndex);
                    if (colorStart > 0 && colorStart < primaryIndex + 50)
                    {
                        var colorEnd = branding.PaletteJson.IndexOf("\"", colorStart + 1);
                        if (colorEnd > colorStart)
                            primaryColor = branding.PaletteJson.Substring(colorStart, colorEnd - colorStart);
                    }
                }
            }
        }

        var dto = new EntitySettingsResponseDto
        {
            TenantId = tenant.Id,
            PrimaryColor = primaryColor,
            LogoUrl = branding?.LogoUrl ?? string.Empty,
            UnderwriterDigitalSignatureUrl = tenant.UnderwriterDigitalSignatureUrl,
            UnderwriterName = tenant.UnderwriterName
        };

        return Result<EntitySettingsResponseDto>.Success(dto);
    }

    public async Task<Result<EntitySettingsResponseDto>> UpdateAsync(UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<EntitySettingsResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<EntitySettingsResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<EntitySettingsResponseDto>.Failed("Unauthorized access.");

        var tenantId = isSuperAdmin ? user.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId))
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null)
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        // Update underwriter fields
        if (!string.IsNullOrWhiteSpace(dto.UnderwriterDigitalSignatureUrl))
            tenant.UnderwriterDigitalSignatureUrl = dto.UnderwriterDigitalSignatureUrl;

        if (!string.IsNullOrWhiteSpace(dto.UnderwriterName))
            tenant.UnderwriterName = dto.UnderwriterName;

        db.Tenants.Update(tenant);
        await db.SaveChangesAsync(cancellationToken);

        // Reload branding
        var branding = await db.CompanyBrandings
            .FirstOrDefaultAsync(b => b.TenantId == tenantId, cancellationToken);

        var primaryColor = "#000000";
        if (branding != null && !string.IsNullOrEmpty(branding.PaletteJson))
        {
            var primaryIndex = branding.PaletteJson.IndexOf("\"primary\"");
            if (primaryIndex > 0)
            {
                var colorStart = branding.PaletteJson.IndexOf("#", primaryIndex);
                if (colorStart > 0 && colorStart < primaryIndex + 50)
                {
                    var colorEnd = branding.PaletteJson.IndexOf("\"", colorStart + 1);
                    if (colorEnd > colorStart)
                        primaryColor = branding.PaletteJson.Substring(colorStart, colorEnd - colorStart);
                }
            }
        }

        var responseDto = new EntitySettingsResponseDto
        {
            TenantId = tenant.Id,
            PrimaryColor = primaryColor,
            LogoUrl = branding?.LogoUrl ?? string.Empty,
            UnderwriterDigitalSignatureUrl = tenant.UnderwriterDigitalSignatureUrl,
            UnderwriterName = tenant.UnderwriterName
        };

        return Result<EntitySettingsResponseDto>.Success(responseDto);
    }
}

