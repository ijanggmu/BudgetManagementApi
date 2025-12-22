using System.Collections.ObjectModel;
using System.Net;
using Business.AdminPortalApi.ExcelExport;
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
using SharedKernel.Constant;
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
    private readonly IExcelExportService _excelExportService;

    public TenantAdminService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        UserManager<ApplicationUser> userManager,
        IMailService mailService,
        IConfiguration configuration,
        ILogger<TenantAdminService> logger,
        IHostEnvironment hostEnvironment,
        IExcelExportService excelExportService)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _userManager = userManager;
        _mailService = mailService;
        _configuration = configuration;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _excelExportService = excelExportService;
    }

    public async Task<Result<List<TenantsResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default)
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

    public async Task<Result<TenantResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var tenant = await _db.Tenants
            .Include(x => x.Branding)
            .AsNoTracking()
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();

        if (tenant == null)
            return Result<TenantResponseDto>.Failed("Tenant not found.");

        BrandingResponseDto? branding = null;
        if (tenant.Branding != null)
        {
            branding = new BrandingResponseDto(
                tenant.Branding.TenantId,
                tenant.Branding.LogoUrl,
                tenant.Branding.PaletteJson,
                tenant.Branding.TypographyJson,
                tenant.Branding.Version,
                tenant.Branding.CreatedOn
            );
        }

        var response = new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.IsActive,
            tenant.Branding?.Version ?? 1,
            tenant.CreatedOn,
            branding
        );

        return Result<TenantResponseDto>.Success(response);
    }

    public async Task<Result<TenantsResponseDto>> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
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
                Version = dto.ThemeVersion > 0 ? dto.ThemeVersion : dto.CompanyBranding.Version
            };

            await _db.CompanyBrandings.AddAsync(companyBranding);

            // Create admin user with password from frontend
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.AdminUser.Username,
                Email = dto.AdminUser.Email,
                EmailConfirmed = true, // Auto-confirm since password is provided
                IsDisabled = false,
                TenantId = tenant.Id // Set tenant ID for the user
            };

            // Create user with password from frontend
            var createUserResult = await _userManager.CreateAsync(adminUser, dto.AdminUser.Password);
            _logger.LogInformation("Created tenant admin user {Username} with password from frontend", dto.AdminUser.Username);

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

            // Send welcome email to admin user
            try
            {
                // Get frontend URL from configuration
                var frontendUrl = _configuration["FrontEndUrlOptions:AgentPortal"]
                    ?? _configuration["FrontendUrl:AgentPortal"]
                    ?? "https://localhost:3000";

                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Welcome to {dto.Name}!</h2>
                            <p>Hello {dto.AdminUser.FullName},</p>
                            <p>Your tenant admin account has been created successfully. You can now log in with your credentials.</p>
                            <p style='margin: 30px 0;'>
                                <a href='{frontendUrl.TrimEnd('/')}/login' 
                                   style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    Login to Your Account
                                </a>
                            </p>
                            <p style='color: #7f8c8d; font-size: 12px;'>
                                If you did not request this account, please contact support.
                            </p>
                        </div>
                    </body>
                    </html>";

                var mailRequest = new MailRequest(
                    to: new Collection<string> { adminUser.Email },
                    subject: $"Welcome to {dto.Name} - Admin Account Created",
                    emailType: "TenantAdminWelcome",
                    body: emailBody
                );

                _mailService.QueueEmail(mailRequest, CancellationToken.None);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the tenant creation
                _logger.LogError(ex, "Failed to send welcome email to {Email}", adminUser.Email);
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

    public async Task<Result<TenantsResponseDto>> UpdateAsync(string id, UpdateTenantDto dto, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
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

    public async Task<Result<List<TenantDropdownDto>>> GetTenantsForDropdownAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenants = await _db.Set<Tenant>()
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new TenantDropdownDto(t.Id, t.Name, t.Slug))
                .ToListAsync();

            return Result<List<TenantDropdownDto>>.Success(tenants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants for dropdown: {Message}", ex.Message);
            return Result<List<TenantDropdownDto>>.Failed($"An error occurred while retrieving tenants: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            requestModel.PageSize = -1;
            var result = await ListAsync(requestModel, cancellationToken);

            if (!result.IsSuccess || result.Data == null)
                return Result<byte[]>.Failed(result.Error ?? "Failed to retrieve tenant data.");

            var columnMappings = new Dictionary<string, string>
            {
                { "Id", "ID" },
                { "Name", "Name" },
                { "Slug", "Slug" },
                { "IsActive", "Is Active" },
                { "ThemeVersion", "Theme Version" },
                { "CreatedOn", "Created On" }
            };

            var excelData = await _excelExportService.ExportToExcelAsync(result.Data, "Tenants", columnMappings);
            return Result<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting tenants to Excel: {Message}", ex.Message);
            return Result<byte[]>.Failed($"An error occurred while exporting: {ex.Message}");
        }
    }
}
