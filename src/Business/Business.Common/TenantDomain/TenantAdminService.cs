using Business.AdminPortalApi.ExcelExport;
using Business.Common.File;
using Business.Common.Mail;
using Common.Mail;
using Data.Context;
using Data.Seed;
using Data.Entities.AdminEntity;
using Data.Entities.CustomerEntity;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using Tenant = Data.Entities.Tenant.Tenant;
using TenantAddress = Data.Entities.Tenant.Address;

namespace Business.Common.TenantDomain;

public class TenantAdminService : ITenantAdminService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantAdminService> _logger;
    private readonly IExcelExportService _excelExportService;
    private readonly IFileService _fileService;
    private readonly IUserProfileService _userProfileService;

    public TenantAdminService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMailService mailService,
        IConfiguration configuration,
        ILogger<TenantAdminService> logger,
        IExcelExportService excelExportService,
        IFileService fileService,
        IUserProfileService userProfileService)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _userManager = userManager;
        _roleManager = roleManager;
        _mailService = mailService;
        _configuration = configuration;
        _logger = logger;
        _excelExportService = excelExportService;
        _fileService = fileService;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<TenantsResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Tenants
            .Include(t => t.Branding)
            .AsNoTracking();

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);

        var tenants = await result.Select(t => new TenantsResponseDto(
            t.Id,
            t.Name,
            t.Slug,
            t.IsActive,
            t.Branding.Version,
            t.CreatedOn,
            t.CurrencyCode,
            t.TimeZoneId
        )).ToListAsync(cancellationToken: cancellationToken);

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };
        return Result<List<TenantsResponseDto>>.Success(tenants, pagination);
    }

    public async Task<Result<TenantResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var tenant = await _db.Tenants
            .Include(x => x.Branding)
            .AsNoTracking()
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (tenant == null)
            return Result<TenantResponseDto>.Failed("Tenant not found.");

        BrandingResponseDto branding = null;
        if (tenant.Branding != null)
        {
            branding = new BrandingResponseDto(
                tenant.Branding.TenantId,
                tenant.Branding.LogoUrl,
                null,
                tenant.Branding.PaletteJson,
                tenant.Branding.TypographyJson,
                tenant.Branding.Version,
                tenant.Branding.CreatedOn
            );
        }
        if (!string.IsNullOrEmpty(branding.LogoUrl))
        {
            var logoUrl = await _fileService.GetFilePresignedUrlAsync(branding.LogoUrl);
            branding = branding with { LogoPath = logoUrl };
        }

        var response = new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.IsActive,
            tenant.Branding?.Version ?? 1,
            tenant.CreatedOn,
            tenant.CurrencyCode,
            tenant.TimeZoneId,
            branding
        );

        return Result<TenantResponseDto>.Success(response);
    }

    public async Task<Result<TenantsResponseDto>> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (await _db.Tenants.AnyAsync(t => t.Slug == dto.Slug, cancellationToken: cancellationToken))
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
                CurrencyCode = dto.CurrencyCode,
                TimeZoneId = dto.TimeZoneId
            };

            await _db.Tenants.AddAsync(tenant, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken); // Save to get tenant ID

            var companyBranding = new CompanyBranding
            {
                TenantId = tenant.Id,
                LogoUrl = dto.CompanyBranding.LogoUrl,
                PaletteJson = dto.CompanyBranding.PaletteJson,
                TypographyJson = dto.CompanyBranding.TypographyJson,
                Version = dto.CompanyBranding.Version
            };


            await _db.CompanyBrandings.AddAsync(companyBranding, cancellationToken);

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

            var rolesName = await SeedTenantRolesAsync(tenant.Id, tenant.Slug);
            await SeedTenantDefaultRolesAsync(tenant.Id,tenant.Slug);
            await SeedTenantAdminPermissions(rolesName.AdminRoleName, tenant.Id, cancellationToken);
            await SeedTenantCeoCfoHodMenuPermissionsAsync(tenant.Id, tenant.Slug, cancellationToken);
            // Must link to this tenant's seeded role Admin-{slug}, not the global "Admin" role.
            // AddToRoleAsync uses RoleManager.FindByNameAsync under global query filters (SuperAdmin has no
            // tenant context), so only TenantId == null roles are visible — wrongly matching "Admin".
            var tenantAdminRole = await _db.Roles
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.TenantId == tenant.Id && r.Name == rolesName.AdminRoleName && !r.IsDeleted,
                    cancellationToken);

            if (tenantAdminRole == null)
            {
                await _userManager.DeleteAsync(adminUser);
                return Result<TenantsResponseDto>.Failed(
                    $"Could not resolve tenant admin role '{rolesName.AdminRoleName}' for this tenant.");
            }

            await _db.UserRoles.AddAsync(
                new ApplicationUserRoles
                {
                    UserId = adminUser.Id,
                    RoleId = tenantAdminRole.Id,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                cancellationToken);

            var stampResult = await _userManager.UpdateSecurityStampAsync(adminUser);
            if (!stampResult.Succeeded)
            {
                await _userManager.DeleteAsync(adminUser);
                return Result<TenantsResponseDto>.Failed(
                    $"Failed to update user after role assignment: {string.Join(", ", stampResult.Errors.Select(e => e.Description))}");
            }

            // Create Admin entity
            var admin = new Admin
            {
                FullName = dto.AdminUser.FullName,
                UserId = adminUser.Id
            };
            await _db.Admins.AddAsync(admin, cancellationToken);

            // Seed tenant-specific roles (Admin) for this tenant

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

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
                    to: [adminUser.Email],
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
                tenant.CreatedOn,
                tenant.CurrencyCode,
                tenant.TimeZoneId
            );

            return Result<TenantsResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating tenant: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<Result<TenantsResponseDto>> UpdateAsync(string id, UpdateTenantDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _db.Tenants
                .Include(t => t.Branding)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken: cancellationToken);
            if (existing == null)
                return Result<TenantsResponseDto>.Failed("Tenant not found.");

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != existing.Slug)
            {
                if (await _db.Tenants.AnyAsync(t => t.Slug == dto.Slug && t.Id != id, cancellationToken: cancellationToken))
                    return Result<TenantsResponseDto>.Failed("Tenant with this slug already exists.");
                existing.Slug = dto.Slug;
            }

            if (!string.IsNullOrEmpty(dto.Name))
                existing.Name = dto.Name;

            if (dto.IsActive.HasValue)
                existing.IsActive = dto.IsActive.Value;

            if (!string.IsNullOrEmpty(dto.CurrencyCode))
                existing.CurrencyCode = dto.CurrencyCode;

            if (!string.IsNullOrEmpty(dto.TimeZoneId))
                existing.TimeZoneId = dto.TimeZoneId;

            existing.Branding.Version = dto.CompanyBranding.Version;
            existing.Branding.LogoUrl = dto.CompanyBranding.LogoUrl;
            existing.Branding.PaletteJson = dto.CompanyBranding.PaletteJson;
            existing.Branding.TypographyJson = dto.CompanyBranding.TypographyJson;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new TenantsResponseDto(
                existing.Id,
                existing.Name,
                existing.Slug,
                existing.IsActive,
                existing.Branding.Version,
                existing.CreatedOn,
                existing.CurrencyCode,
                existing.TimeZoneId
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

    public async Task<Result<bool>> DeleteAsync(string id, DeleteTenantDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Password))
            return Result<bool>.Failed("Password is required to delete a tenant.");

        var roleIdClaim = _userProfileService.GetRoleId();
        if (string.IsNullOrEmpty(roleIdClaim) ||
            !roleIdClaim.Contains(SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            return Result<bool>.Failed("Only SuperAdmin can delete tenants.");

        var currentUserId = _userProfileService.GetUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Result<bool>.Failed("User context is missing.");

        var superAdminUser = await _userManager.FindByIdAsync(currentUserId);
        if (superAdminUser == null || superAdminUser.IsDeleted)
            return Result<bool>.Failed("User account not found.");

        if (!await _userManager.CheckPasswordAsync(superAdminUser, dto.Password))
            return Result<bool>.Failed("The password you entered is incorrect.");

        var tenantId = id;
        var existing = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (existing == null)
            return Result<bool>.Failed("Tenant not found.");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await PurgeTenantScopedDataAsync(tenantId, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            _logger.LogWarning("Tenant {TenantId} and all related data were permanently deleted by SuperAdmin {UserId}", tenantId, currentUserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error deleting tenant {TenantId}: {Message}", tenantId, ex.Message);
            return Result<bool>.Failed($"Could not delete the tenant: {ex.Message}");
        }
    }

    /// <summary>Removes all rows scoped to the tenant, then identity roles/users, then the tenant record.</summary>
    private async Task PurgeTenantScopedDataAsync(string tenantId, CancellationToken cancellationToken)
    {
        // Tenant-scoped business data (respect FK order)
        await _db.BudgetMemoAuditLogs.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Memos.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.BudgetRequests.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.ApprovalConfigs.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Budgets.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.UserSignatures.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.MemoTemplates.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.BudgetSubheadings.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.BudgetHeadings.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.NepaliFiscalYears.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Departments.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Branches.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Designations.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.NotificationHistories.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Notifications.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Noticeboards.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.AttendanceEntries.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.EmailGatewayConfigurations.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.SmsGatewayConfigurations.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);

        await _db.Prospects.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Contacts.IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        await _db.Set<TenantAddress>().IgnoreQueryFilters().Where(x => x.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);

        var customerIds = await _db.Customers.IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);
        if (customerIds.Count > 0)
        {
            await _db.Addresses.IgnoreQueryFilters()
                .Where(a => customerIds.Contains(a.CustomerId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        await _db.Customers.IgnoreQueryFilters().Where(c => c.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);

        var userIds = await _db.Users.IgnoreQueryFilters()
            .Where(u => u.TenantId == tenantId)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        await _db.Admins.IgnoreQueryFilters()
            .Where(a => a.TenantId == tenantId || userIds.Contains(a.UserId))
            .ExecuteDeleteAsync(cancellationToken);

        if (userIds.Count > 0)
        {
            await _db.UserOtps.IgnoreQueryFilters()
                .Where(o => userIds.Contains(o.UserId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        var tenantRoleIds = await _db.Roles.IgnoreQueryFilters()
            .Where(r => r.TenantId == tenantId)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        if (tenantRoleIds.Count > 0 || userIds.Count > 0)
        {
            await _db.UserRoles.IgnoreQueryFilters()
                .Where(ur =>
                    (userIds.Count > 0 && userIds.Contains(ur.UserId)) ||
                    (tenantRoleIds.Count > 0 && tenantRoleIds.Contains(ur.RoleId)))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (tenantRoleIds.Count > 0)
        {
            await _db.RoleClaims.IgnoreQueryFilters()
                .Where(rc => tenantRoleIds.Contains(rc.RoleId))
                .ExecuteDeleteAsync(cancellationToken);

            foreach (var roleId in tenantRoleIds)
            {
                var role = await _db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
                if (role == null)
                    continue;
                var del = await _roleManager.DeleteAsync(role);
                if (!del.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to delete role {roleId}: {string.Join(", ", del.Errors.Select(e => e.Description))}");
            }
        }

        foreach (var uid in userIds)
        {
            var user = await _userManager.FindByIdAsync(uid);
            if (user == null)
                continue;
            var delUser = await _userManager.DeleteAsync(user);
            if (!delUser.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to delete user {uid}: {string.Join(", ", delUser.Errors.Select(e => e.Description))}");
        }

        var branding = await _db.CompanyBrandings.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.TenantId == tenantId, cancellationToken);
        if (branding != null)
            _db.CompanyBrandings.Remove(branding);

        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant != null)
            _db.Tenants.Remove(tenant);

        await _db.SaveChangesAsync(cancellationToken);
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

    /// <summary>
    /// Seeds tenant-specific Admin role for a tenant.
    /// </summary>
    private async Task<SeedRoleResponseModel> SeedTenantRolesAsync(
    string tenantId,
    string tenantName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantName);

        var adminRoleName = $"{SystemRoles.Admin}-{tenantName}";

        var exists = await _db.Roles
            .AnyAsync(r => r.TenantId == tenantId && r.Name == adminRoleName);

        if (!exists)
        {
            var adminRole = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = adminRoleName,
                RoleDisplayName = "Tenant Administrator",
                Description = $"Tenant Admin Role for {tenantId}",
                RoleLevel = SystemRoles.AdminLevel,
                RoleType = SystemRoles.Admin,
                TenantId = tenantId
            };

            var result = await _roleManager.CreateAsync(adminRole);

            if (!result.Succeeded)
            {
                _logger.LogError(
                    "Failed to create Admin role for tenant {TenantId}. Errors: {Errors}",
                    tenantId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new InvalidOperationException(
                    $"Admin role creation failed for tenant {tenantId}");
            }

            _logger.LogInformation(
                "Created Admin role {RoleName} for tenant {TenantId}",
                adminRoleName,
                tenantId);
        }

        // Always return the canonical name so permissions + user assignment work even if the role already existed.
        return new SeedRoleResponseModel(AdminRoleName: adminRoleName);
    }

    /// <summary>
    /// Seeds default tenant business roles (CEO, CFO, HOD, HodAssistance) for a newly created tenant.
    /// </summary>
    private async Task SeedTenantDefaultRolesAsync(string tenantId, string tenantName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        var tenantRoles = SystemRoles.GetTenantDefaultRoles();

        foreach (var roleName in tenantRoles)
        {
            var roleNameStored = $"{roleName}-{tenantName}";

            var normalizedName = _roleManager.NormalizeKey(roleNameStored);
            var exists = await _db.Roles
                .AnyAsync(r => r.TenantId == tenantId && r.NormalizedName == normalizedName);

            if (exists)
                continue;

            var role = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleNameStored,
                NormalizedName = normalizedName,
                RoleDisplayName = GetTenantRoleDisplayName(roleName),
                Description = roleName,
                RoleLevel = SystemRoles.CEOCFOHODLevel,
                RoleType = roleName,
                TenantId = tenantId
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                _logger.LogError(
                    "Failed to create role {RoleName} for tenant {TenantId}. Errors: {Errors}",
                    roleName,
                    tenantId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new InvalidOperationException(
                    $"Role {roleName} creation failed for tenant {tenantId}");
            }

            _logger.LogInformation(
                "Created role {RoleName} for tenant {TenantId}",
                roleName,
                tenantId);
        }
    }

    private static string GetTenantRoleDisplayName(string roleName)
    {
        return roleName switch
        {
            SystemRoles.CEO => "Chief Executive Officer",
            SystemRoles.CFO => "Chief Financial Officer",
            SystemRoles.HOD => "Head of Department",
            SystemRoles.HodAssistance => "HOD Assistant",
            _ => roleName
        };
    }

    private async Task SeedTenantAdminPermissions(string roleName, string tenantId, CancellationToken cancellationToken = default)
    {
        var roleId = await _db.Roles
            .IgnoreQueryFilters()
            .Where(r => r.Name == roleName && r.TenantId == tenantId && !r.IsDeleted)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(roleId))
            return;

        await MenuPermissionSeeder.UpsertRoleMenuPermissionsAsync(
            _db,
            roleId,
            MenuPermissionSeeder.GetAdminPermissions(),
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Seeds menu permissions for tenant-scoped CEO, CFO, HOD, and HodAssistance roles.</summary>
    private async Task SeedTenantCeoCfoHodMenuPermissionsAsync(string tenantId, string tenantSlug, CancellationToken cancellationToken)
    {
        foreach (var roleType in SystemRoles.GetTenantDefaultRoles())
        {
            var storedName = $"{roleType}-{tenantSlug}";
            var roleId = await _db.Roles
                .IgnoreQueryFilters()
                .Where(r => r.TenantId == tenantId && r.Name == storedName && !r.IsDeleted)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogWarning(
                    "Skipping menu permissions: role {RoleName} not found for tenant {TenantId}",
                    storedName,
                    tenantId);
                continue;
            }

            var permissions = roleType switch
            {
                SystemRoles.CEO => MenuPermissionSeeder.GetCEOPermissions(),
                SystemRoles.CFO => MenuPermissionSeeder.GetCFOPermissions(),
                SystemRoles.HOD => MenuPermissionSeeder.GetHODPermissions(),
                SystemRoles.HodAssistance => MenuPermissionSeeder.GetHodAssistancePermissions(),
                _ => (IReadOnlyList<string>)Array.Empty<string>()
            };

            if (permissions.Count == 0)
                continue;

            await MenuPermissionSeeder.UpsertRoleMenuPermissionsAsync(_db, roleId, permissions, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public record SeedRoleResponseModel(string AdminRoleName);

}
