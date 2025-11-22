using System;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BrandingService : IBrandingService
{
    private readonly ApplicationDataContext _db;
    private readonly ITenantContext _tenant;
    private readonly IUserProfileService _userProfileService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<BrandingService> _logger;

    public BrandingService(
        ApplicationDataContext db,
        ITenantContext tenant,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager,
        ILogger<BrandingService> logger)
    {
        _db = db;
        _tenant = tenant;
        _userProfileService = userProfileService;
        _userManager = userManager;
        _logger = logger;
    }

    private static BrandingResponseDto MapToDto(CompanyBranding branding)
    {
        return new BrandingResponseDto(
            branding.TenantId,
            branding.LogoUrl,
            branding.PaletteJson,
            branding.TypographyJson,
            branding.Version,
            branding.CreatedOn
        );
    }

    public async Task<Result<BrandingResponseDto>> GetAsync()
    {
        try
        {
            if (_tenant.TenantId is null)
                return Result<BrandingResponseDto>.Failed("Tenant not resolved.");

            var branding = await _db.Set<CompanyBranding>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == _tenant.TenantId);

            if (branding is null)
                return Result<BrandingResponseDto>.Failed("Branding not found for current tenant.");

            return Result<BrandingResponseDto>.Success(MapToDto(branding));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branding: {Message}", ex.Message);
            return Result<BrandingResponseDto>.Failed($"An error occurred while retrieving branding: {ex.Message}");
        }
    }

    public async Task<Result<BrandingResponseDto>> GetByTenantIdAsync(string tenantId)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<BrandingResponseDto>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<BrandingResponseDto>.Failed("User not found or inactive.");

            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(_db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync();

            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

            // Only SuperAdmin can access branding by tenantId
            if (!isSuperAdmin)
                return Result<BrandingResponseDto>.Failed("Access denied. SuperAdmin role required to access branding by tenantId.");

            // Verify tenant exists
            var tenant = await _db.Set<Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

            if (tenant == null)
                return Result<BrandingResponseDto>.Failed("Tenant not found or inactive.");

            var branding = await _db.Set<CompanyBranding>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (branding is null)
                return Result<BrandingResponseDto>.Failed("Branding not found for the specified tenant.");

            return Result<BrandingResponseDto>.Success(MapToDto(branding));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branding by tenantId: {Message}", ex.Message);
            return Result<BrandingResponseDto>.Failed($"An error occurred while retrieving branding: {ex.Message}");
        }
    }

    public async Task<Result<BrandingResponseDto>> UpdateAsync(UpdateBrandingDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            if (_tenant.TenantId is null)
                return Result<BrandingResponseDto>.Failed("Tenant not resolved.");

            var branding = await _db.Set<CompanyBranding>()
                .FirstOrDefaultAsync(x => x.TenantId == _tenant.TenantId);

            if (branding is null)
            {
                branding = new CompanyBranding { TenantId = _tenant.TenantId };
                _db.Add(branding);
            }

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(dto.LogoUrl))
                branding.LogoUrl = dto.LogoUrl;

            if (!string.IsNullOrWhiteSpace(dto.PaletteJson))
                branding.PaletteJson = dto.PaletteJson;

            if (!string.IsNullOrWhiteSpace(dto.TypographyJson))
                branding.TypographyJson = dto.TypographyJson;

            branding.Version += 1;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<BrandingResponseDto>.Success(MapToDto(branding));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating branding: {Message}", ex.Message);
            return Result<BrandingResponseDto>.Failed($"An error occurred while updating branding: {ex.Message}");
        }
    }
}


