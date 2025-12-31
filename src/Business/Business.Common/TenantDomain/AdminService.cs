using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.AdminPortalApi.ExcelExport;
using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using Tenant = Data.Entities.Tenant.Tenant;

namespace Business.Common.TenantDomain;

public class AdminService(
    ApplicationDataContext db,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension,
    IExcelExportService excelExportService)
    : IAdminService
{
    public async Task<Result<List<AdminResponseDto>>> GetAdminsForAdminAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        // We only need to check if user is SuperAdmin for query filtering logic
        
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        IQueryable<Admin> query = db.Admins
            .Include(a => a.User);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        // For SuperAdmin: filter by tenantId if provided, otherwise show all
        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }
            // If tenantId is null, show all admins (global query filter will be ignored)
            query = query.IgnoreQueryFilters();
        }
        // For Admin: only show admins from their tenant (global query filter applies)

        var admins = await query.ToListAsync(cancellationToken);

        // Get tenant names for all unique tenant IDs
        var tenantIds = admins.Where(a => !string.IsNullOrEmpty(a.TenantId)).Select(a => a.TenantId).Distinct().ToList();
        var tenants = await db.Tenants.Where(t => tenantIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var dtos = new List<AdminResponseDto>();
        foreach (var admin in admins)
        {
            var userRoles = await userManager.GetRolesAsync(admin.User);
            var tenantName = !string.IsNullOrEmpty(admin.TenantId) && tenants.ContainsKey(admin.TenantId) 
                ? tenants[admin.TenantId] 
                : string.Empty;

            dtos.Add(new AdminResponseDto(
                admin.Id,
                admin.FullName,
                admin.User.Email ?? string.Empty,
                admin.User.PhoneNumber,
                admin.User.UserName ?? string.Empty,
                admin.UserId,
                admin.TenantId ?? string.Empty,
                tenantName,
                userRoles.ToList(),
                admin.User.IsDisabled,
                admin.User.EmailConfirmed,
                admin.CreatedOn
            ));
        }

        return Result<List<AdminResponseDto>>.Success(dtos);
    }

    public async Task<Result<AdminResponseDto>> GetAdminByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var query = db.Admins
            .Include(a => a.User)
            .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var admin = await query.FirstOrDefaultAsync(cancellationToken);

        if (admin == null)
            return Result<AdminResponseDto>.Failed("Admin not found.");

        var userRoles = await userManager.GetRolesAsync(admin.User);
        var tenantName = string.Empty;
        if (!string.IsNullOrEmpty(admin.TenantId))
        {
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == admin.TenantId, cancellationToken);
            tenantName = tenant?.Name ?? string.Empty;
        }

        var dto = new AdminResponseDto(
            admin.Id,
            admin.FullName,
            admin.User.Email ?? string.Empty,
            admin.User.PhoneNumber,
            admin.User.UserName ?? string.Empty,
            admin.UserId,
            admin.TenantId ?? string.Empty,
            tenantName,
            userRoles.ToList(),
            admin.User.IsDisabled,
            admin.User.EmailConfirmed,
            admin.CreatedOn
        );

        return Result<AdminResponseDto>.Success(dto);
    }

    public async Task<Result<AdminResponseDto>> CreateAsync(CreateAdminDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            // Get tenant ID from current user (for Admin) or from context
            var userId = userProfileService.GetUserId();
            var user = await userManager.FindByIdAsync(userId);
            var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;

            // Check if username already exists
            // Note: IsDeleted filter is now applied globally
            var usernameExists = await db.Users
                .AnyAsync(u => u.UserName == dto.Username, cancellationToken);

            if (usernameExists)
                return Result<AdminResponseDto>.Failed("Username already exists.");

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                // Note: IsDeleted filter is now applied globally
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email, cancellationToken);

                if (emailExists)
                    return Result<AdminResponseDto>.Failed("Email already exists.");
            }

            // Validate roles if provided
            if (dto.Roles != null && dto.Roles.Any())
            {
                // Note: IsDeleted filter is now applied globally
                var validRoles = await roleManager.Roles
                    .Where(r => dto.Roles.Contains(r.Name))
                    .Select(r => r.Name)
                    .ToListAsync(cancellationToken);

                if (validRoles.Count != dto.Roles.Count)
                    return Result<AdminResponseDto>.Failed("One or more roles are invalid.");

                // Tenant admins can only assign Admin role, not SuperAdmin
                if (!isSuperAdmin && dto.Roles.Any(r => r == SystemRoles.SuperAdmin))
                    return Result<AdminResponseDto>.Failed("You cannot assign SuperAdmin role.");
            }

            // Create ApplicationUser
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = true,
                IsDisabled = false,
                TenantId = tenantId
            };

            var createUserResult = await userManager.CreateAsync(adminUser, dto.Password);
            if (!createUserResult.Succeeded)
                return Result<AdminResponseDto>.Failed(createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user.");

            // Assign roles - default to Admin if no roles specified
            var rolesToAssign = dto.Roles != null && dto.Roles.Any() 
                ? dto.Roles 
                : new List<string> { SystemRoles.Admin };

            foreach (var roleName in rolesToAssign)
            {
                var roleResult = await userManager.AddToRoleAsync(adminUser, roleName);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return Result<AdminResponseDto>.Failed(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign role.");
                }
            }

            // Create Admin entity
            var admin = new Admin
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName.Trim(),
                UserId = adminUser.Id,
                TenantId = tenantId
            };

            await db.Admins.AddAsync(admin, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var userRoles = await userManager.GetRolesAsync(adminUser);
            var tenantName = string.Empty;
            if (!string.IsNullOrEmpty(tenantId))
            {
                var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
                tenantName = tenant?.Name ?? string.Empty;
            }

            var responseDto = new AdminResponseDto(
                admin.Id,
                admin.FullName,
                adminUser.Email ?? string.Empty,
                adminUser.PhoneNumber,
                adminUser.UserName ?? string.Empty,
                adminUser.Id,
                admin.TenantId ?? string.Empty,
                tenantName,
                userRoles.ToList(),
                adminUser.IsDisabled,
                adminUser.EmailConfirmed,
                admin.CreatedOn
            );

            return Result<AdminResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<AdminResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AdminResponseDto>> UpdateAsync(string id, UpdateAdminDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            var query = db.Admins
                .Include(a => a.User)
                .Where(a => a.Id == id);
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var admin = await query.FirstOrDefaultAsync(cancellationToken);

            if (admin == null)
                return Result<AdminResponseDto>.Failed("Admin not found.");

            // Update admin properties
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                admin.FullName = dto.FullName.Trim();

            // Update user properties
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != admin.User.Email)
            {
                // Note: IsDeleted filter is now applied globally
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && u.Id != admin.UserId, cancellationToken);

                if (emailExists)
                    return Result<AdminResponseDto>.Failed("Email already exists.");

                admin.User.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != admin.User.PhoneNumber)
            {
                // Note: IsDeleted filter is now applied globally
                var phoneExists = await db.Users
                    .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber && u.Id != admin.UserId, cancellationToken);

                if (phoneExists)
                    return Result<AdminResponseDto>.Failed("Phone number already exists.");

                admin.User.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.IsDisabled.HasValue)
                admin.User.IsDisabled = dto.IsDisabled.Value;

            // Update roles if provided
            if (dto.Roles != null)
            {
                // Validate roles
                // Note: IsDeleted filter is now applied globally
                var validRoles = await roleManager.Roles
                    .Where(r => dto.Roles.Contains(r.Name))
                    .Select(r => r.Name)
                    .ToListAsync(cancellationToken);

                if (validRoles.Count != dto.Roles.Count)
                    return Result<AdminResponseDto>.Failed("One or more roles are invalid.");

                // Tenant admins cannot assign SuperAdmin role
                if (!isSuperAdmin && dto.Roles.Any(r => r == SystemRoles.SuperAdmin))
                    return Result<AdminResponseDto>.Failed("You cannot assign SuperAdmin role.");

                // Get current roles
                var currentRoles = await userManager.GetRolesAsync(admin.User);
                
                // Remove roles that are not in the new list
                var rolesToRemove = currentRoles.Except(dto.Roles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(admin.User, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<AdminResponseDto>.Failed("Failed to remove roles.");
                    }
                }

                // Add new roles
                var rolesToAdd = dto.Roles.Except(currentRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await userManager.AddToRolesAsync(admin.User, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<AdminResponseDto>.Failed("Failed to add roles.");
                    }
                }
            }

            var updateResult = await userManager.UpdateAsync(admin.User);
            if (!updateResult.Succeeded)
                return Result<AdminResponseDto>.Failed(updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user.");

            db.Admins.Update(admin);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var userRoles = await userManager.GetRolesAsync(admin.User);
            var tenantName = string.Empty;
            if (!string.IsNullOrEmpty(admin.TenantId))
            {
                var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == admin.TenantId, cancellationToken);
                tenantName = tenant?.Name ?? string.Empty;
            }

            var responseDto = new AdminResponseDto(
                admin.Id,
                admin.FullName,
                admin.User.Email ?? string.Empty,
                admin.User.PhoneNumber,
                admin.User.UserName ?? string.Empty,
                admin.UserId,
                admin.TenantId ?? string.Empty,
                tenantName,
                userRoles.ToList(),
                admin.User.IsDisabled,
                admin.User.EmailConfirmed,
                admin.CreatedOn
            );

            return Result<AdminResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<AdminResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var userId = userProfileService.GetUserId();
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            var query = db.Admins
                .Include(a => a.User)
                .Where(a => a.Id == id);
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var admin = await query.FirstOrDefaultAsync(cancellationToken);

            if (admin == null)
                return Result<bool>.Failed("Admin not found.");

            // Prevent deleting yourself
            if (admin.UserId == userId)
                return Result<bool>.Failed("You cannot delete your own account.");

            // Soft delete
            admin.IsDeleted = true;
            admin.User.IsDeleted = true;

            db.Admins.Update(admin);
            db.Users.Update(admin.User);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            // Extract tenantId from Sieve filters if present, otherwise null (will filter based on user role)
            string? tenantId = null;
            if (!string.IsNullOrEmpty(requestModel.Filters) && requestModel.Filters.Contains("TenantId=="))
            {
                var tenantIdMatch = System.Text.RegularExpressions.Regex.Match(requestModel.Filters, @"TenantId==([^,|]+)");
                if (tenantIdMatch.Success)
                    tenantId = tenantIdMatch.Groups[1].Value.Trim();
            }

            var result = await GetAdminsForAdminAsync(tenantId, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
                return Result<byte[]>.Failed(result.Error ?? "Failed to retrieve admin data.");

            var columnMappings = new Dictionary<string, string>
            {
                { "Id", "ID" },
                { "FullName", "Full Name" },
                { "Email", "Email" },
                { "PhoneNumber", "Phone Number" },
                { "Username", "Username" },
                { "UserId", "User ID" },
                { "TenantId", "Tenant ID" },
                { "TenantName", "Tenant Name" },
                { "Roles", "Roles" },
                { "IsDisabled", "Is Disabled" },
                { "EmailConfirmed", "Email Confirmed" },
                { "CreatedOn", "Created On" }
            };

            var excelData = await excelExportService.ExportToExcelAsync(result.Data, "Admins", columnMappings);
            return Result<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failed($"An error occurred while exporting: {ex.Message}");
        }
    }
}

