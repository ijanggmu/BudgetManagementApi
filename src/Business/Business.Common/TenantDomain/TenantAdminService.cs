using System.Collections.ObjectModel;
using System.Net;
using Business.Common.Mail;
using Common.Mail;
using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using Tenant = Data.Entities.Tenant.Tenant;

namespace Business.Common.TenantDomain;

public class TenantAdminService : ITenantAdminService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantAdminService> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    // Static password for development environment
    private const string DevelopmentPassword = "Admin@123";

    public TenantAdminService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        UserManager<ApplicationUser> userManager,
        IMailService mailService,
        IConfiguration configuration,
        ILogger<TenantAdminService> logger,
        IHostEnvironment hostEnvironment)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _userManager = userManager;
        _mailService = mailService;
        _configuration = configuration;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<Result<List<TenantsResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null)
    {
        var query = _db.Set<Tenant>()
            .Include(t => t.Branding)
            .AsNoTracking()
            .OrderBy(t => t.Name);

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var tenants = await result.Select(t => new TenantsResponseDto(
                t.Id,
                t.Name,
                t.Slug,
                t.IsActive,
                t.Branding.Version,
                t.CreatedOn
            )).ToListAsync();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };
            return Result<List<TenantsResponseDto>>.Success(tenants, pagination);
        }

        var allTenants = await query.Select(t => new TenantsResponseDto(
            t.Id,
            t.Name,
            t.Slug,
            t.IsActive,
            t.Branding.Version,
            t.CreatedOn
        )).ToListAsync();

        return Result<List<TenantsResponseDto>>.Success(allTenants);
    }

    public async Task<Result<TenantsResponseDto>> GetByIdAsync(string id)
    {
        var tenant = await _db.Tenants.Include(x => x.Branding).AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TenantsResponseDto(
                t.Id,
                t.Name,
                t.Slug,
                t.IsActive,
                t.Branding.Version,
                t.CreatedOn
            ))
            .FirstOrDefaultAsync();

        if (tenant == null)
            return Result<TenantsResponseDto>.Failed("Tenant not found.");

        return Result<TenantsResponseDto>.Success(tenant);
    }

    public async Task<Result<TenantsResponseDto>> CreateAsync(CreateTenantDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            if (await _db.Tenants.AnyAsync(t => t.Slug == dto.Slug))
                return Result<TenantsResponseDto>.Failed("Tenant with this slug already exists.");

            // Check if admin user email/username already exists
            if (await _userManager.FindByEmailAsync(dto.AdminUser.Email) != null)
                return Result<TenantsResponseDto>.Failed("Admin user with this email already exists.");

            if (await _userManager.FindByNameAsync(dto.AdminUser.Username) != null)
                return Result<TenantsResponseDto>.Failed("Admin user with this username already exists.");

            var tenant = new Tenant
            {
                Name = dto.Name,
                Slug = dto.Slug,
                IsActive = dto.IsActive,
            };

            await _db.Tenants.AddAsync(tenant);
            await _db.SaveChangesAsync(); // Save to get tenant ID

            var companyBranding = new CompanyBranding
            {
                TenantId = tenant.Id,
                LogoUrl = dto.CompanyBranding.LogoUrl,
                PaletteJson = dto.CompanyBranding.PaletteJson,
                TypographyJson = dto.CompanyBranding.TypographyJson,
                Version = dto.CompanyBranding.Version
            };

            await _db.CompanyBrandings.AddAsync(companyBranding);

            // Determine if we're in development environment
            var isDevelopment = _hostEnvironment.IsDevelopment() 
                || _hostEnvironment.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase)
                || _hostEnvironment.EnvironmentName.Equals("Dev", StringComparison.OrdinalIgnoreCase);

            // Create admin user
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.AdminUser.Username,
                Email = dto.AdminUser.Email,
                EmailConfirmed = isDevelopment, // Auto-confirm in development, require activation in production
                IsDisabled = false,
                TenantId = tenant.Id // Set tenant ID for the user
            };

            // Create user with or without password based on environment
            IdentityResult createUserResult;
            if (isDevelopment)
            {
                // In development: create user with static password
                createUserResult = await _userManager.CreateAsync(adminUser, DevelopmentPassword);
                _logger.LogInformation("Development mode: Created tenant admin user {Username} with static password", dto.AdminUser.Username);
            }
            else
            {
                // In production: create user without password (will be set during activation)
                createUserResult = await _userManager.CreateAsync(adminUser);
            }

            if (!createUserResult.Succeeded)
            {
                return Result<TenantsResponseDto>.Failed(
                    $"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
            }

            // Add Admin role
            var addRoleResult = await _userManager.AddToRoleAsync(adminUser, SystemRoles.Admin);
            if (!addRoleResult.Succeeded)
            {
                // Rollback user creation if role assignment fails
                await _userManager.DeleteAsync(adminUser);
                return Result<TenantsResponseDto>.Failed(
                    $"Failed to assign Admin role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }

            // Create Admin entity
            var admin = new Admin
            {
                FullName = dto.AdminUser.FullName,
                UserId = adminUser.Id
            };
            await _db.Admins.AddAsync(admin);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Only send activation email in production environments
            if (!isDevelopment)
            {
                // Generate password reset token for activation (valid for 7 days)
                var activationToken = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
                var encodedToken = WebUtility.UrlEncode(activationToken);

                // Get frontend URL from configuration
                var frontendUrl = _configuration["FrontEndUrlOptions:AgentPortal"]
                    ?? _configuration["FrontendUrl:AgentPortal"]
                    ?? "https://localhost:3000";

                // Build activation link
                var activationLink = $"{frontendUrl.TrimEnd('/')}/activate-account?token={encodedToken}&email={WebUtility.UrlEncode(adminUser.Email)}";

                // Send activation email
                try
                {
                    var emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #2c3e50;'>Welcome to {dto.Name}!</h2>
                                <p>Hello {dto.AdminUser.FullName},</p>
                                <p>Your tenant admin account has been created successfully. To activate your account and set your password, please click the link below:</p>
                                <p style='margin: 30px 0;'>
                                    <a href='{activationLink}' 
                                       style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Activate Account & Set Password
                                    </a>
                                </p>
                                <p style='color: #7f8c8d; font-size: 12px;'>
                                    This link will expire in 7 days. If you did not request this account, please contact support.
                                </p>
                                <p style='color: #7f8c8d; font-size: 12px; margin-top: 30px;'>
                                    If the button doesn't work, copy and paste this link into your browser:<br/>
                                    <a href='{activationLink}' style='color: #3498db; word-break: break-all;'>{activationLink}</a>
                                </p>
                            </div>
                        </body>
                        </html>";

                    var mailRequest = new MailRequest(
                        to: new Collection<string> { adminUser.Email },
                        subject: $"Activate Your {dto.Name} Admin Account",
                        emailType: "TenantAdminActivation",
                        body: emailBody
                    );

                    _mailService.QueueEmail(mailRequest, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the tenant creation
                    _logger.LogError(ex, "Failed to send activation email to {Email}", adminUser.Email);
                }
            }
            else
            {
                _logger.LogInformation("Development mode: Skipping activation email for tenant admin {Email}. Password: {Password}", 
                    adminUser.Email, DevelopmentPassword);
            }

            // Reload tenant with branding to get the version
            await _db.Entry(tenant).Reference(t => t.Branding).LoadAsync();

            var response = new TenantsResponseDto(
                tenant.Id,
                tenant.Name,
                tenant.Slug,
                tenant.IsActive,
                tenant.Branding?.Version ?? 1,
                tenant.CreatedOn
            );

            return Result<TenantsResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating tenant: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<Result<TenantsResponseDto>> UpdateAsync(string id, UpdateTenantDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _db.Set<Tenant>()
                .Include(t => t.Branding)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return Result<TenantsResponseDto>.Failed("Tenant not found.");

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != existing.Slug)
            {
                if (await _db.Set<Tenant>().AnyAsync(t => t.Slug == dto.Slug && t.Id != id))
                    return Result<TenantsResponseDto>.Failed("Tenant with this slug already exists.");
                existing.Slug = dto.Slug;
            }

            if (!string.IsNullOrEmpty(dto.Name))
                existing.Name = dto.Name;

            if (dto.IsActive.HasValue)
                existing.IsActive = dto.IsActive.Value;

            if (dto.ThemeVersion.HasValue && existing.Branding != null)
                existing.Branding.Version = dto.ThemeVersion.Value;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            var response = new TenantsResponseDto(
                existing.Id,
                existing.Name,
                existing.Slug,
                existing.IsActive,
                existing.Branding.Version,
                existing.CreatedOn
            );

            return Result<TenantsResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating tenant {TenantId}: {Message}", id, ex.Message);
            throw;
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _db.Set<Tenant>().FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return Result<bool>.Failed("Tenant not found.");

            _db.Remove(existing);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting tenant {TenantId}: {Message}", id, ex.Message);
            throw;
        }
    }
}
