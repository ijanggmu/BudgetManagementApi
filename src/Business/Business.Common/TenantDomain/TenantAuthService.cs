using System.Net;
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

public class TenantAuthService : ITenantAuthService
{
    private readonly ApplicationDataContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IUserProfileService _userProfileService;

    public TenantAuthService(
        ApplicationDataContext db,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IUserProfileService userProfileService)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _userProfileService = userProfileService;
    }

    public async Task<Result<TenantLoginResponseDto>> LoginAsync(TenantLoginRequestDto request)
    {
        // First, verify the tenant exists and is active
        var tenant = await _db.Set<Tenant>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == request.Slug && t.IsActive);

        if (tenant == null)
            return Result<TenantLoginResponseDto>.Failed("Invalid tenant or tenant is inactive.");

        // Find user by username
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || user.IsDeleted)
            return Result<TenantLoginResponseDto>.Failed("Invalid username or password.");

        // Check if user has Admin role
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(SystemRoles.Admin) && !roles.Contains(SystemRoles.SuperAdmin))
            return Result<TenantLoginResponseDto>.Failed("User does not have tenant admin access.");

        // Verify password
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (signInResult == SignInResult.Failed)
        {
            user.AccessFailedCount++;
            await _userManager.UpdateAsync(user);
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
        var tokenModel = _tokenService.CreateToken(user, roles.ToList());
        var refresh = await _tokenService.CreateRefreshToken(user);

        var response = new TenantLoginResponseDto(
            tokenModel.Item1,
            tokenModel.Item2,
            refresh.Item1,
            refresh.Item2,
            tenant.Id,
            tenant.Name
        );

        // Set cookies
        var tokenResult = new TokenModel
        {
            AccessToken = tokenModel.Item1,
            AccessTokenExpiryInSeconds = tokenModel.Item2,
            RefreshToken = refresh.Item1,
            RefreshTokenExpiryInSeconds = refresh.Item2
        };

        _userProfileService.SetAuthCookiesInClient(tokenResult, request.Username);

        return Result<TenantLoginResponseDto>.Success(response, statusCode: HttpStatusCode.NoContent);
    }
}

