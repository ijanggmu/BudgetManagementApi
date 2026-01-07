using Business.Common.File;
using Data.Context;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Entity;
using Models.Common;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class EntitySettingsService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService,
    IFileService fileService)
    : IEntitySettingsService
{
    public async Task<Result<EntitySettingsResponseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);

        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;

        if (string.IsNullOrEmpty(tenantId))
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var tenant = await db.Tenants
            .Include(x => x.Branding)
            .Where(t => t.Id == tenantId)
            .AsNoTracking()
            .Select(x => new EntitySettingsResponseDto
            {
                TenantId = x.Id,
                PaletteJson = x.Branding != null ? x.Branding.PaletteJson : string.Empty,
                LogoUrl = x.Branding != null ? x.Branding.LogoUrl : string.Empty,
                UnderwriterDigitalSignatureUrl = x.UnderwriterDigitalSignatureUrl,
                UnderwriterName = x.UnderwriterName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var underwriterDigitalSignatureSignedUrl = string.IsNullOrEmpty(tenant.UnderwriterDigitalSignatureUrl)
            ? string.Empty
            : await fileService.GetFilePresignedUrlAsync(tenant.UnderwriterDigitalSignatureUrl);

        var logoSignedUrl = string.IsNullOrEmpty(tenant.LogoUrl)
            ? string.Empty
            : await fileService.GetFilePresignedUrlAsync(tenant.LogoUrl);

        tenant.UnderwriterDigitalSignatureSignedUrl = underwriterDigitalSignatureSignedUrl;
        tenant.LogoUrl = logoSignedUrl;

        return Result<EntitySettingsResponseDto>.Success(tenant);
    }

    public async Task<Result<MessageResponseModel>> UpdateAsync(UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId))
            return Result<MessageResponseModel>.Failed("Tenant not found.");

        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null)
            return Result<MessageResponseModel>.Failed("Tenant not found.");

        // Update underwriter fields
        if (!string.IsNullOrWhiteSpace(dto.UnderwriterDigitalSignatureUrl))
            tenant.UnderwriterDigitalSignatureUrl = dto.UnderwriterDigitalSignatureUrl;

        if (!string.IsNullOrWhiteSpace(dto.UnderwriterName))
            tenant.UnderwriterName = dto.UnderwriterName;

        db.Tenants.Update(tenant);
        await db.SaveChangesAsync(cancellationToken);


        return Result<MessageResponseModel>.Success(new MessageResponseModel("Organization Updated Successfully."));
    }
}

