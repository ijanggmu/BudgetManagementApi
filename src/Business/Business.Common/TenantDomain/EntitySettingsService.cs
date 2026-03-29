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
        var isSuperAdmin = userProfileService.IsSuperAdmin();

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
                PanNumber = x.PanNumber ?? string.Empty,
                PhoneNumber = x.PhoneNumber ?? string.Empty,
                Address = x.Address ?? string.Empty,
                CompanyStampUrl = x.CompanyStampUrl ?? string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
            return Result<EntitySettingsResponseDto>.Failed("Tenant not found.");

        var logoSignedUrl = string.IsNullOrEmpty(tenant.LogoUrl)
            ? string.Empty
            : await fileService.GetFilePresignedUrlAsync(tenant.LogoUrl);

        var companyStampSignedUrl = string.IsNullOrEmpty(tenant.CompanyStampUrl)
            ? string.Empty
            : await fileService.GetFilePresignedUrlAsync(tenant.CompanyStampUrl);

        tenant.LogoUrl = logoSignedUrl;
        tenant.CompanyStampSignedUrl = companyStampSignedUrl;

        return Result<EntitySettingsResponseDto>.Success(tenant);
    }

    public async Task<Result<MessageResponseModel>> UpdateAsync(UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId))
            return Result<MessageResponseModel>.Failed("Tenant not found.");

        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null)
            return Result<MessageResponseModel>.Failed("Tenant not found.");

        // Update underwriter and organization fields

        if (dto.PanNumber != null)
            tenant.PanNumber = dto.PanNumber;

        if (dto.PhoneNumber != null)
            tenant.PhoneNumber = dto.PhoneNumber;

        if (dto.Address != null)
            tenant.Address = dto.Address;

        if (!string.IsNullOrWhiteSpace(dto.CompanyStampUrl))
            tenant.CompanyStampUrl = dto.CompanyStampUrl;

        db.Tenants.Update(tenant);
        await db.SaveChangesAsync(cancellationToken);


        return Result<MessageResponseModel>.Success(new MessageResponseModel("Organization Updated Successfully."));
    }
}

