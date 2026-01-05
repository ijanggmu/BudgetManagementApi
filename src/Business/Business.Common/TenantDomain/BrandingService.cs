using Business.Common.File;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BrandingService(
    ApplicationDataContext db,
    ITenantContext tenant,
    IUserProfileService userProfileService,
    UserManager<ApplicationUser> userManager,
    ILogger<BrandingService> logger,
    IFileService fileService) : IBrandingService
{
    public async Task<Result<BrandingResponseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (tenant.TenantId is null)
                return Result<BrandingResponseDto>.Failed("Tenant not resolved.");

            var branding = await db.CompanyBrandings
                .AsNoTracking()
                .Select(branding => new BrandingResponseDto(
                                branding.TenantId,
                                branding.LogoUrl,
                                null,
                                branding.PaletteJson,
                                branding.TypographyJson,
                                branding.Version,
                                branding.CreatedOn
                            ))
                .FirstOrDefaultAsync(x => x.TenantId == tenant.TenantId, cancellationToken);

            if (branding is null)
                return Result<BrandingResponseDto>.Failed("Branding not found for current tenant.");

            if (!string.IsNullOrWhiteSpace(branding.LogoUrl))
            {
                var presignedUrl = await fileService
                    .GetFilePresignedUrlAsync(branding.LogoUrl);

                branding = branding with
                {
                    LogoPath = presignedUrl
                };
            }
            return Result<BrandingResponseDto>.Success(branding);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving branding: {Message}", ex.Message);
            return Result<BrandingResponseDto>.Failed($"An error occurred while retrieving branding: {ex.Message}");
        }
    }

    public async Task<Result<BrandingResponseDto>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<BrandingResponseDto>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<BrandingResponseDto>.Failed("User not found or inactive.");

            var userRoles = await db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync(cancellationToken);

            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

            // Only SuperAdmin can access branding by tenantId
            if (!isSuperAdmin)
                return Result<BrandingResponseDto>.Failed("Access denied. SuperAdmin role required to access branding by tenantId.");

            // Verify tenant exists
            var tenant = await db.Set<Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);

            if (tenant == null)
                return Result<BrandingResponseDto>.Failed("Tenant not found or inactive.");

            var branding = await db.CompanyBrandings
                .Where(x => x.TenantId == tenantId)
                .AsNoTracking()
                .Select(branding => new BrandingResponseDto(
                                        branding.TenantId,
                                        branding.LogoUrl,
                                        null,
                                        branding.PaletteJson,
                                        branding.TypographyJson,
                                        branding.Version,
                                        branding.CreatedOn))
                .FirstOrDefaultAsync(cancellationToken);

            if (branding is null)
                return Result<BrandingResponseDto>.Failed("Branding not found for the specified tenant.");

            if (!string.IsNullOrWhiteSpace(branding.LogoUrl))
            {
                var presignedUrl = await fileService
                    .GetFilePresignedUrlAsync(branding.LogoUrl);

                branding = branding with
                {
                    LogoPath = presignedUrl
                };
            }

            return Result<BrandingResponseDto>.Success(branding);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving branding by tenantId: {Message}", ex.Message);
            return Result<BrandingResponseDto>.Failed($"An error occurred while retrieving branding: {ex.Message}");
        }
    }

    public async Task<Result<MessageResponseModel>> UpdateAsync(UpdateBrandingDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (tenant.TenantId is null)
                return Result<MessageResponseModel>.Failed("Tenant not resolved.");

            var branding = await db.CompanyBrandings
                .FirstOrDefaultAsync(x => x.TenantId == tenant.TenantId, cancellationToken);

            if (branding is null)
            {
                branding = new CompanyBranding { TenantId = tenant.TenantId };
                db.Add(branding);
            }

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(dto.LogoUrl))
                branding.LogoUrl = dto.LogoUrl;

            if (!string.IsNullOrWhiteSpace(dto.PaletteJson))
                branding.PaletteJson = dto.PaletteJson;

            if (!string.IsNullOrWhiteSpace(dto.TypographyJson))
                branding.TypographyJson = dto.TypographyJson;

            branding.Version += 1;

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Brand Updated Successfully."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error updating branding: {Message}", ex.Message);
            throw;
        }
    }
}


