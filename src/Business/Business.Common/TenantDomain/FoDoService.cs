using System.Threading;
using Data.Context;
using Data.Entities.Audit.UserActivites;
using Data.Entities.FodoEntity;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class FodoService(
    ApplicationDataContext db,
    AuditDataContext auditContext,
    UserManager<ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IFodoService
{
    public async Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(string tenantId = null, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<List<FodoResponseDto>>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<List<FodoResponseDto>>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<List<FodoResponseDto>>.Failed("Unauthorized access.");

        IQueryable<Fodo> query = db.Fodos
            .Include(a => a.User)
            .Include(a => a.Designation)
            .Include(a => a.Branch)
            .Where(a => !a.IsDeleted && !a.User.IsDeleted);

        // For SuperAdmin: filter by tenantId if provided, otherwise show all
        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }
            // If tenantId is null, show all fodos (global query filter will be ignored)
            query = query.IgnoreQueryFilters();
        }
        // For Admin: only show fodos from their tenant (global query filter applies)

        var fodos = await query.ToListAsync(cancellationToken);

        // Get tenant names for all unique tenant IDs
        var tenantIds = fodos.Where(a => !string.IsNullOrEmpty(a.TenantId)).Select(a => a.TenantId).Distinct().ToList();
        var tenants = await db.Tenants.Where(t => tenantIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var dtos = fodos.Select(a => new FodoResponseDto
        {
            Id = a.Id,
            FullName = a.FullName,
            EmployeeId = a.EmployeeId ?? string.Empty,
            Email = a.User.Email ?? string.Empty,
            PhoneNumber = a.User.PhoneNumber ?? string.Empty,
            DesignationId = a.DesignationId,
            DesignationTitle = a.Designation?.Title,
            BranchId = a.BranchId,
            BranchName = a.Branch?.BranchName,
            PermanentProvince = a.PermanentProvince,
            PermanentDistrict = a.PermanentDistrict,
            PermanentMunicipality = a.PermanentMunicipality,
            PermanentWard = a.PermanentWard,
            TemporaryProvince = a.TemporaryProvince,
            TemporaryDistrict = a.TemporaryDistrict,
            TemporaryMunicipality = a.TemporaryMunicipality,
            TemporaryWard = a.TemporaryWard,
            IsActive = a.IsActive,
            IsDisabled = a.User.IsDisabled,
            TenantId = a.TenantId ?? string.Empty,
            TenantName = !string.IsNullOrEmpty(a.TenantId) && tenants.ContainsKey(a.TenantId) ? tenants[a.TenantId] : string.Empty,
            UserId = a.UserId,
            Username = a.User.UserName ?? string.Empty,
            CreatedOn = a.CreatedOn
        }).ToList();

        return Result<List<FodoResponseDto>>.Success(dtos);
    }

    public async Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<FodoResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<FodoResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Fodos
            .Include(a => a.User)
            .Include(a => a.Designation)
            .Include(a => a.Branch)
            .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var fodo = await query.FirstOrDefaultAsync(cancellationToken);

        if (fodo == null)
            return Result<FodoResponseDto>.Failed("Fodo not found.");

        var tenantName = string.Empty;
        if (!string.IsNullOrEmpty(fodo.TenantId))
        {
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == fodo.TenantId, cancellationToken);
            tenantName = tenant?.Name ?? string.Empty;
        }

        var dto = new FodoResponseDto
        {
            Id = fodo.Id,
            FullName = fodo.FullName,
            EmployeeId = fodo.EmployeeId ?? string.Empty,
            Email = fodo.User.Email ?? string.Empty,
            PhoneNumber = fodo.User.PhoneNumber ?? string.Empty,
            DesignationId = fodo.DesignationId,
            DesignationTitle = fodo.Designation?.Title,
            BranchId = fodo.BranchId,
            BranchName = fodo.Branch?.BranchName,
            PermanentProvince = fodo.PermanentProvince,
            PermanentDistrict = fodo.PermanentDistrict,
            PermanentMunicipality = fodo.PermanentMunicipality,
            PermanentWard = fodo.PermanentWard,
            TemporaryProvince = fodo.TemporaryProvince,
            TemporaryDistrict = fodo.TemporaryDistrict,
            TemporaryMunicipality = fodo.TemporaryMunicipality,
            TemporaryWard = fodo.TemporaryWard,
            IsActive = fodo.IsActive,
            IsDisabled = fodo.User.IsDisabled,
            TenantId = fodo.TenantId ?? string.Empty,
            TenantName = tenantName,
            UserId = fodo.UserId,
            Username = fodo.User.UserName ?? string.Empty,
            CreatedOn = fodo.CreatedOn
        };

        return Result<FodoResponseDto>.Success(dto);
    }

    public async Task<Result<FodoResponseDto>> CreateAsync(CreateFodoDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<FodoResponseDto>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<FodoResponseDto>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
            var isAdmin = roles.Contains(SystemRoles.Admin);

            if (!isSuperAdmin && !isAdmin)
                return Result<FodoResponseDto>.Failed("Unauthorized access.");

            // Get tenant ID from current user (for Admin) or from context
            var tenantId = isSuperAdmin ? currentUser.TenantId : db.CurrentTenantId;

            // Check if phone number already exists
            var phoneExists = await db.Users
                .AnyAsync(u => u.PhoneNumber == dto.MobileNumber && !u.IsDeleted, cancellationToken);

            if (phoneExists)
                return Result<FodoResponseDto>.Failed("Mobile number already exists.");

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

                if (emailExists)
                    return Result<FodoResponseDto>.Failed("Email already exists.");
            }

            // Get country dialing code
            var country = await db.Countries
                .FirstOrDefaultAsync(c => c.Id == dto.CountryId, cancellationToken);

            if (country == null)
                return Result<FodoResponseDto>.Failed("Invalid country ID.");

            var username = $"{country.CountryDialingCode}{dto.MobileNumber}";

            // Check if username already exists
            var usernameExists = await db.Users
                .AnyAsync(u => u.UserName == username && !u.IsDeleted, cancellationToken);

            if (usernameExists)
                return Result<FodoResponseDto>.Failed("Username already exists.");

            // Check if EmployeeId already exists
            if (!string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                var employeeIdExists = await db.Fodos
                    .AnyAsync(f => f.EmployeeId == dto.EmployeeId && !f.IsDeleted, cancellationToken);

                if (employeeIdExists)
                    return Result<FodoResponseDto>.Failed("Employee ID already exists.");
            }

            // Validate Designation if provided
            if (!string.IsNullOrWhiteSpace(dto.DesignationId))
            {
                var designationExists = await db.Designations
                    .AnyAsync(d => d.Id == dto.DesignationId && !d.IsDeleted, cancellationToken);

                if (!designationExists)
                    return Result<FodoResponseDto>.Failed("Invalid Designation ID.");
            }

            // Validate Branch if provided
            if (!string.IsNullOrWhiteSpace(dto.BranchId))
            {
                var branchExists = await db.Branches
                    .AnyAsync(b => b.Id == dto.BranchId && !b.IsDeleted, cancellationToken);

                if (!branchExists)
                    return Result<FodoResponseDto>.Failed("Invalid Branch ID.");
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = username,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                PhoneNumber = dto.MobileNumber,
                PhoneCountryId = dto.CountryId,
                LockoutEnabled = true,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TenantId = tenantId
            };

            var createUserResult = await userManager.CreateAsync(user, dto.Password);
            if (!createUserResult.Succeeded)
                return Result<FodoResponseDto>.Failed(createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user.");

            // Add Agent role
            var roleResult = await userManager.AddToRoleAsync(user, SystemRoles.FoDo);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<FodoResponseDto>.Failed(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign role.");
            }

            // Create Fodo entity
            var fodo = new Fodo
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName.Trim(),
                EmployeeId = dto.EmployeeId?.Trim(),
                UserId = user.Id,
                DesignationId = string.IsNullOrWhiteSpace(dto.DesignationId) ? null : dto.DesignationId,
                BranchId = string.IsNullOrWhiteSpace(dto.BranchId) ? null : dto.BranchId,
                PermanentProvince = dto.PermanentProvince?.Trim(),
                PermanentDistrict = dto.PermanentDistrict?.Trim(),
                PermanentMunicipality = dto.PermanentMunicipality?.Trim(),
                PermanentWard = dto.PermanentWard,
                TemporaryProvince = dto.TemporaryProvince?.Trim(),
                TemporaryDistrict = dto.TemporaryDistrict?.Trim(),
                TemporaryMunicipality = dto.TemporaryMunicipality?.Trim(),
                TemporaryWard = dto.TemporaryWard,
                IsActive = dto.IsActive,
                TenantId = tenantId
            };

            await db.Fodos.AddAsync(fodo, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Reload with includes for response
            await db.Entry(fodo).Reference(x => x.Designation).LoadAsync(cancellationToken);
            await db.Entry(fodo).Reference(x => x.Branch).LoadAsync(cancellationToken);

            var responseDto = new FodoResponseDto
            {
                Id = fodo.Id,
                FullName = fodo.FullName,
                EmployeeId = fodo.EmployeeId ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DesignationId = fodo.DesignationId,
                DesignationTitle = fodo.Designation?.Title,
                BranchId = fodo.BranchId,
                BranchName = fodo.Branch?.BranchName,
                PermanentProvince = fodo.PermanentProvince,
                PermanentDistrict = fodo.PermanentDistrict,
                PermanentMunicipality = fodo.PermanentMunicipality,
                PermanentWard = fodo.PermanentWard,
                TemporaryProvince = fodo.TemporaryProvince,
                TemporaryDistrict = fodo.TemporaryDistrict,
                TemporaryMunicipality = fodo.TemporaryMunicipality,
                TemporaryWard = fodo.TemporaryWard,
                IsActive = fodo.IsActive,
                IsDisabled = user.IsDisabled,
                TenantId = fodo.TenantId ?? string.Empty,
                UserId = fodo.UserId,
                Username = user.UserName ?? string.Empty,
                CreatedOn = fodo.CreatedOn
            };

            return Result<FodoResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<FodoResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<FodoResponseDto>> UpdateAsync(string id, UpdateFodoDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<FodoResponseDto>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<FodoResponseDto>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Include(a => a.Designation)
                .Include(a => a.Branch)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<FodoResponseDto>.Failed("Fodo not found.");

            // Check if EmployeeId already exists (if changed)
            if (!string.IsNullOrWhiteSpace(dto.EmployeeId) && dto.EmployeeId != fodo.EmployeeId)
            {
                var employeeIdExists = await db.Fodos
                    .AnyAsync(f => f.EmployeeId == dto.EmployeeId && f.Id != id && !f.IsDeleted, cancellationToken);

                if (employeeIdExists)
                    return Result<FodoResponseDto>.Failed("Employee ID already exists.");
            }

            // Validate Designation if provided
            if (!string.IsNullOrWhiteSpace(dto.DesignationId))
            {
                var designationExists = await db.Designations
                    .AnyAsync(d => d.Id == dto.DesignationId && !d.IsDeleted, cancellationToken);

                if (!designationExists)
                    return Result<FodoResponseDto>.Failed("Invalid Designation ID.");
            }

            // Validate Branch if provided
            if (!string.IsNullOrWhiteSpace(dto.BranchId))
            {
                var branchExists = await db.Branches
                    .AnyAsync(b => b.Id == dto.BranchId && !b.IsDeleted, cancellationToken);

                if (!branchExists)
                    return Result<FodoResponseDto>.Failed("Invalid Branch ID.");
            }

            // Update fodo properties
            fodo.FullName = dto.FullName.Trim();
            fodo.EmployeeId = dto.EmployeeId?.Trim();
            fodo.DesignationId = string.IsNullOrWhiteSpace(dto.DesignationId) ? null : dto.DesignationId;
            fodo.BranchId = string.IsNullOrWhiteSpace(dto.BranchId) ? null : dto.BranchId;
            fodo.PermanentProvince = dto.PermanentProvince?.Trim();
            fodo.PermanentDistrict = dto.PermanentDistrict?.Trim();
            fodo.PermanentMunicipality = dto.PermanentMunicipality?.Trim();
            fodo.PermanentWard = dto.PermanentWard;
            fodo.TemporaryProvince = dto.TemporaryProvince?.Trim();
            fodo.TemporaryDistrict = dto.TemporaryDistrict?.Trim();
            fodo.TemporaryMunicipality = dto.TemporaryMunicipality?.Trim();
            fodo.TemporaryWard = dto.TemporaryWard;
            fodo.IsActive = dto.IsActive;

            // Update user properties
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != fodo.User.Email)
            {
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && u.Id != fodo.UserId && !u.IsDeleted, cancellationToken);

                if (emailExists)
                    return Result<FodoResponseDto>.Failed("Email already exists.");

                fodo.User.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.MobileNumber) && dto.MobileNumber != fodo.User.PhoneNumber)
            {
                var phoneExists = await db.Users
                    .AnyAsync(u => u.PhoneNumber == dto.MobileNumber && u.Id != fodo.UserId && !u.IsDeleted, cancellationToken);

                if (phoneExists)
                    return Result<FodoResponseDto>.Failed("Mobile number already exists.");

                fodo.User.PhoneNumber = dto.MobileNumber;
            }

            var updateResult = await userManager.UpdateAsync(fodo.User);
            if (!updateResult.Succeeded)
                return Result<FodoResponseDto>.Failed(updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user.");

            db.Fodos.Update(fodo);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var responseDto = new FodoResponseDto
            {
                Id = fodo.Id,
                FullName = fodo.FullName,
                EmployeeId = fodo.EmployeeId ?? string.Empty,
                Email = fodo.User.Email ?? string.Empty,
                PhoneNumber = fodo.User.PhoneNumber ?? string.Empty,
                DesignationId = fodo.DesignationId,
                DesignationTitle = fodo.Designation?.Title,
                BranchId = fodo.BranchId,
                BranchName = fodo.Branch?.BranchName,
                PermanentProvince = fodo.PermanentProvince,
                PermanentDistrict = fodo.PermanentDistrict,
                PermanentMunicipality = fodo.PermanentMunicipality,
                PermanentWard = fodo.PermanentWard,
                TemporaryProvince = fodo.TemporaryProvince,
                TemporaryDistrict = fodo.TemporaryDistrict,
                TemporaryMunicipality = fodo.TemporaryMunicipality,
                TemporaryWard = fodo.TemporaryWard,
                IsActive = fodo.IsActive,
                IsDisabled = fodo.User.IsDisabled,
                TenantId = fodo.TenantId ?? string.Empty,
                UserId = fodo.UserId,
                Username = fodo.User.UserName ?? string.Empty,
                CreatedOn = fodo.CreatedOn
            };

            return Result<FodoResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<FodoResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<bool>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<bool>.Failed("Fodo not found.");

            // Soft delete
            fodo.IsDeleted = true;
            fodo.User.IsDeleted = true;

            db.Fodos.Update(fodo);
            db.Users.Update(fodo.User);
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

    public async Task<Result<bool>> ChangePasswordAsync(string id, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<bool>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<bool>.Failed("Marketing Executive not found.");

            var token = await userManager.GeneratePasswordResetTokenAsync(fodo.User);
            var result = await userManager.ResetPasswordAsync(fodo.User, token, newPassword);

            if (!result.Succeeded)
                return Result<bool>.Failed(result.Errors.FirstOrDefault()?.Description ?? "Failed to change password.");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ToggleUserStatusAsync(string id, bool isDisabled, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<bool>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<bool>.Failed("Marketing Executive not found.");

            fodo.User.IsDisabled = isDisabled;
            var result = await userManager.UpdateAsync(fodo.User);

            if (!result.Succeeded)
                return Result<bool>.Failed(result.Errors.FirstOrDefault()?.Description ?? "Failed to update user status.");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<object>>> GetAccessLogsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<object>>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<List<object>>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Where(a => a.Id == id && !a.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<List<object>>.Failed("Marketing Executive not found.");

            // Get access logs for the user
            var logsQuery = auditContext.UserActivities
                .Where(l => l.UserName == fodo.User.UserName)
                .OrderByDescending(l => l.At);

            var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(logsQuery, requestModel);
            var logs = await result.ToListAsync(cancellationToken);

            var logDtos = logs.Select(l => new
            {
                l.Id,
                l.UserName,
                l.IpAddress,
                l.RequestPath,
                l.RequestMethod,
                l.ResponseStatusCode,
                ResponseTimeInMS = l.ResponseTime,
                l.CorrelationId,
                l.Module,
                At = l.At.ToString(),
                EndAt = l.EndAt.ToString(),
                l.RequestHost,
                l.UserAgent
            }).Cast<object>().ToList();

            return Result<List<object>>.Success(logDtos);
        }
        catch (Exception ex)
        {
            return Result<List<object>>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<object>>> GetLeadsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<object>>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<List<object>>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<List<object>>.Failed("Marketing Executive not found.");

            // Get leads created by this marketing executive
            var leadsQuery = db.Leads
                .Where(l => l.OwnerUserId == fodo.UserId && !l.IsDeleted);


            if (!isSuperAdmin && !string.IsNullOrEmpty(currentUser.TenantId))
                leadsQuery = leadsQuery.Where(l => l.TenantId == currentUser.TenantId);

            leadsQuery.Include(l => l.Prospect)
                .ThenInclude(p => p.PrimaryContact);

            var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(leadsQuery, requestModel);
            var leads = await result.ToListAsync(cancellationToken);

            var leadDtos = leads.Select(l => new
            {
                l.Id,
                l.Status,
                l.Source,
                ProspectName = l.Prospect?.PrimaryContact?.FullName,
                CreatedOn = l.CreatedOn
            }).Cast<object>().ToList();

            return Result<List<object>>.Success(leadDtos);
        }
        catch (Exception ex)
        {
            return Result<List<object>>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<object>>> GetQuotationsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<object>>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<List<object>>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<List<object>>.Failed("Marketing Executive not found.");

            // Get quotations for leads created by this marketing executive
            var leadIds = await db.Leads
                .Where(l => l.OwnerUserId == fodo.UserId && !l.IsDeleted)
                .Select(l => l.ProspectId)
                .ToListAsync(cancellationToken);

            var quotationsQuery = db.Quotations
                .Where(q => leadIds.Contains(q.ProspectId) && !q.IsDeleted);


            if (!isSuperAdmin && !string.IsNullOrEmpty(currentUser.TenantId))
                quotationsQuery = quotationsQuery.Where(q => q.TenantId == currentUser.TenantId);
            quotationsQuery.Include(q => q.Items);
            var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(quotationsQuery, requestModel);
            var quotations = await result.ToListAsync(cancellationToken);

            var quotationDtos = quotations.Select(q => new
            {
                q.Id,
                q.ProspectId,
                TotalAmount = q.Items?.Sum(i => i.Premium) ?? 0,
                q.CreatedOn
            }).Cast<object>().ToList();

            return Result<List<object>>.Success(quotationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<object>>.Failed($"An error occurred: {ex.Message}");
        }
    }
}

