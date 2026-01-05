using System.Net;
using Business.Common.File;
using Business.Common.Token;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common.Token;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using Tenant = Data.Entities.Tenant.Tenant;

namespace Business.Common.TenantDomain;

public class TenantAuthService(
    ApplicationDataContext db,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IUserProfileService userProfileService,
    IFileService fileService) : ITenantAuthService
{
    public async Task<Result<TenantLoginResponseDto>> LoginAsync(TenantLoginRequestDto request)
    {
        // First, verify the tenant exists and is active
        var tenant = await db.Set<Tenant>()
            .Include(t => t.Branding)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == request.Slug && t.IsActive);

        if (tenant == null)
            return Result<TenantLoginResponseDto>.Failed("Invalid tenant or tenant is inactive.");

        // Find user by username
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null || user.IsDeleted)
            return Result<TenantLoginResponseDto>.Failed("Invalid username or password.");

        // Check if user has Admin role
        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains(SystemRoles.Admin) && !roles.Contains(SystemRoles.SuperAdmin))
            return Result<TenantLoginResponseDto>.Failed("User does not have tenant admin access.");

        // Verify password
        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (signInResult == SignInResult.Failed)
        {
            user.AccessFailedCount++;
            await userManager.UpdateAsync(user);
            return Result<TenantLoginResponseDto>.Failed("Invalid username or password.");
        }

        if (signInResult.IsLockedOut)
            return Result<TenantLoginResponseDto>.Failed("Too many login attempts. Please try again later.");

        if (user.IsDisabled)
            return Result<TenantLoginResponseDto>.Failed("User is disabled. Please contact administrator.");

        if (signInResult.IsNotAllowed)
            return Result<TenantLoginResponseDto>.Failed("User is not allowed to login. Please contact administrator.");

        if (!signInResult.Succeeded)
            return Result<TenantLoginResponseDto>.Failed("Invalid username or password.");

        // Generate tokens
        var tokenModel = tokenService.CreateToken(user, [.. roles]);
        var refresh = await tokenService.CreateRefreshToken(user);

        var presignUrlTenantLogo = string.Empty;
        if (!string.IsNullOrEmpty(tenant.Branding.LogoUrl))
        {
            presignUrlTenantLogo = await fileService.GetFilePresignedUrlAsync(tenant.Branding.LogoUrl);
        }
        // Prepare branding response
        BrandingResponseDto branding = null;
        if (tenant.Branding != null)
        {
            branding = new BrandingResponseDto(
                tenant.Branding.TenantId,
                tenant.Branding.LogoUrl,
                presignUrlTenantLogo,
                tenant.Branding.PaletteJson,
                tenant.Branding.TypographyJson,
                tenant.Branding.Version,
                tenant.Branding.CreatedOn
            );
        }

        var response = new TenantLoginResponseDto(
            tokenModel.Item1,
            tokenModel.Item2,
            refresh.Item1,
            refresh.Item2,
            tenant.Id,
            tenant.Name,
            branding
        );

        // Set cookies
        var tokenResult = new TokenModel
        {
            AccessToken = tokenModel.Item1,
            AccessTokenExpiryInSeconds = tokenModel.Item2,
            RefreshToken = refresh.Item1,
            RefreshTokenExpiryInSeconds = refresh.Item2
        };

        userProfileService.SetAuthCookiesInClient(tokenResult, request.Username);

        return Result<TenantLoginResponseDto>.Success(response, statusCode: HttpStatusCode.NoContent);
    }
}

