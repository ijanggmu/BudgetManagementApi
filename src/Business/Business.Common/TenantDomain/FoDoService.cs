using Data.Context;
using Data.Entities.FodoEntity;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
    public async Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        // Only check role for query filtering logic
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        IQueryable<Fodo> query = db.Fodos
            .Include(a => a.User)
            .Include(a => a.Designation)
            .Include(a => a.Branch);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        // For SuperAdmin, ignore the automatic tenant query filter to see all fodos across all tenants
        if (isSuperAdmin)
        {
            query = query.IgnoreQueryFilters();
        }
        // For Admin: only show fodos from their tenant (global query filter applies)
        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var fodos = await result.ToListAsync(cancellationToken);

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

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<FodoResponseDto>>.Success(dtos, pagination);
    }

    public async Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var query = db.Fodos
            .Include(a => a.User)
            .Include(a => a.Designation)
            .Include(a => a.Branch)
            .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

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
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            // Get tenant ID from current user (for Admin) or from context
            var userId = userProfileService.GetUserId();
            var currentUser = await userManager.FindByIdAsync(userId);
            var tenantId = isSuperAdmin ? currentUser?.TenantId : db.CurrentTenantId;

            // Check if phone number already exists
            // Note: IsDeleted filter is now applied globally
            var phoneExists = await db.Users
                .AnyAsync(u => u.PhoneNumber == dto.MobileNumber, cancellationToken);

            if (phoneExists)
                return Result<FodoResponseDto>.Failed("Mobile number already exists.");

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                // Note: IsDeleted filter is now applied globally
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email, cancellationToken);

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
            // Note: IsDeleted filter is now applied globally
            var usernameExists = await db.Users
                .AnyAsync(u => u.UserName == username, cancellationToken);

            if (usernameExists)
                return Result<FodoResponseDto>.Failed("Username already exists.");

            // Check if EmployeeId already exists
            if (!string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                // Note: IsDeleted filter is now applied globally
                var employeeIdExists = await db.Fodos
                    .AnyAsync(f => f.EmployeeId == dto.EmployeeId, cancellationToken);

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
                .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<FodoResponseDto>.Failed("Fodo not found.");

            // Check if EmployeeId already exists (if changed)
            if (!string.IsNullOrWhiteSpace(dto.EmployeeId) && dto.EmployeeId != fodo.EmployeeId)
            {
                // Note: IsDeleted filter is now applied globally
                var employeeIdExists = await db.Fodos
                    .AnyAsync(f => f.EmployeeId == dto.EmployeeId && f.Id != id, cancellationToken);

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
                // Note: IsDeleted filter is now applied globally
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && u.Id != fodo.UserId, cancellationToken);

                if (emailExists)
                    return Result<FodoResponseDto>.Failed("Email already exists.");

                fodo.User.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.MobileNumber) && dto.MobileNumber != fodo.User.PhoneNumber)
            {
                // Note: IsDeleted filter is now applied globally
                var phoneExists = await db.Users
                    .AnyAsync(u => u.PhoneNumber == dto.MobileNumber && u.Id != fodo.UserId, cancellationToken);

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
                .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

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
                .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

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
                .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

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
                .Where(a => a.Id == id);
        // Note: IsDeleted filter is now applied globally

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
                .Where(a => a.Id == id);
        // Note: IsDeleted filter is now applied globally

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<List<object>>.Failed("Marketing Executive not found.");

            // Get leads created by this marketing executive
            // Note: IsDeleted and TenantId filters are now applied globally via query filters
            var leadsQuery = db.Leads
                .Where(l => l.OwnerUserId == fodo.UserId);

            if (isSuperAdmin)
                leadsQuery = leadsQuery.IgnoreQueryFilters();

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
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var roleId = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id);
        // Note: IsDeleted filter is now applied globally

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<List<object>>.Failed("Marketing Executive not found.");

            // Get quotations for leads created by this marketing executive
            // Note: IsDeleted and TenantId filters are now applied globally via query filters
            var leadIds = await db.Leads
                .Where(l => l.OwnerUserId == fodo.UserId)
                .Select(l => l.ProspectId)
                .ToListAsync(cancellationToken);

            var quotationsQuery = db.Quotations
                .Where(q => leadIds.Contains(q.ProspectId));

            if (isSuperAdmin)
                quotationsQuery = quotationsQuery.IgnoreQueryFilters();
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

    public async Task<Result<ImportResult>> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var userId = userProfileService.GetUserId();
        var currentUser = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? currentUser?.TenantId : db.CurrentTenantId;

        var importResult = new ImportResult();
        var errors = new List<ImportError>();

        try
        {
            var workbook = new XSSFWorkbook(fileStream);
            var sheet = workbook.GetSheetAt(0);

            if (sheet == null || sheet.LastRowNum < 1)
                return Result<ImportResult>.Failed("Excel file is empty or invalid.");

            // Validate header row
            var headerRow = sheet.GetRow(0);
            if (headerRow == null)
                return Result<ImportResult>.Failed("Header row is missing.");

            // Map column indices
            var columnMap = new Dictionary<string, int>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.ToString()?.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(cellValue))
                    columnMap[cellValue] = i;
            }

            // Required columns
            var requiredColumns = new[] { "fullname", "employeeid", "email", "mobilenumber" };
            var missingColumns = requiredColumns.Where(c => !columnMap.ContainsKey(c)).ToList();
            if (missingColumns.Any())
                return Result<ImportResult>.Failed($"Required columns are missing: {string.Join(", ", missingColumns)}");

            // Get all designations and branches for lookup
            // Note: IsDeleted filter is now applied globally
            var designations = await db.Designations
                .Where(d => d.TenantId == tenantId)
                .ToDictionaryAsync(d => d.Title.ToLower(), d => d.Id, cancellationToken);

            var branches = await db.Branches
                .Where(b => b.TenantId == tenantId)
                .ToDictionaryAsync(b => b.BranchCode.ToLower(), b => b.Id, cancellationToken);

            var countries = await db.Countries.ToListAsync(cancellationToken);
            var defaultCountry = countries.FirstOrDefault(c => c.Id == 1) ?? countries.FirstOrDefault();

            // Process data rows
            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var fullName = GetCellValue(row, columnMap, "fullname");
                    var employeeId = GetCellValue(row, columnMap, "employeeid");
                    var email = GetCellValue(row, columnMap, "email");
                    var mobileNumber = GetCellValue(row, columnMap, "mobilenumber");
                    var designationTitle = GetCellValue(row, columnMap, "designationtitle", "designation");
                    var branchCode = GetCellValue(row, columnMap, "branchcode", "branch");
                    var permanentProvince = GetCellValue(row, columnMap, "permanentprovince", "permanent province");
                    var permanentDistrict = GetCellValue(row, columnMap, "permanentdistrict", "permanent district");
                    var permanentMunicipality = GetCellValue(row, columnMap, "permanentmunicipality", "permanent municipality");
                    var permanentWardStr = GetCellValue(row, columnMap, "permanentward", "permanent ward");
                    var temporaryProvince = GetCellValue(row, columnMap, "temporaryprovince", "temporary province");
                    var temporaryDistrict = GetCellValue(row, columnMap, "temporarydistrict", "temporary district");
                    var temporaryMunicipality = GetCellValue(row, columnMap, "temporarymunicipality", "temporary municipality");
                    var temporaryWardStr = GetCellValue(row, columnMap, "temporaryward", "temporary ward");
                    var isActiveStr = GetCellValue(row, columnMap, "isactive", "is active");
                    var password = GetCellValue(row, columnMap, "password");
                    var countryIdStr = GetCellValue(row, columnMap, "countryid", "country id");

                    // Validate required fields
                    var validationErrors = new List<string>();
                    if (string.IsNullOrWhiteSpace(fullName))
                        validationErrors.Add("FullName is required.");
                    if (string.IsNullOrWhiteSpace(employeeId))
                        validationErrors.Add("EmployeeId is required.");
                    if (string.IsNullOrWhiteSpace(email))
                        validationErrors.Add("Email is required.");
                    if (string.IsNullOrWhiteSpace(mobileNumber))
                        validationErrors.Add("MobileNumber is required.");

                    if (validationErrors.Any())
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "Validation",
                            ErrorMessage = string.Join(" ", validationErrors)
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Check if email already exists
                    // Note: IsDeleted filter is now applied globally
                    var emailExists = await db.Users
                        .AnyAsync(u => u.Email == email, cancellationToken);

                    if (emailExists)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "Email",
                            ErrorMessage = $"Email '{email}' already exists."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Check if employee ID already exists
                    // Note: IsDeleted filter is now applied globally
                    var employeeIdExists = await db.Fodos
                        .AnyAsync(f => f.EmployeeId == employeeId, cancellationToken);

                    if (employeeIdExists)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "EmployeeId",
                            ErrorMessage = $"Employee ID '{employeeId}' already exists."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Get country ID
                    var country = new Data.Entities.Common.Country();

                    int countryId = defaultCountry?.Id ?? 1;
                    if (!string.IsNullOrWhiteSpace(countryIdStr) && int.TryParse(countryIdStr, out var parsedCountryId))
                    {
                        country = countries.FirstOrDefault(c => c.Id == parsedCountryId);
                        if (country != null)
                            countryId = parsedCountryId;
                    }

                    country = countries.FirstOrDefault(c => c.Id == countryId) ?? defaultCountry;
                    if (country == null)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "CountryId",
                            ErrorMessage = "Invalid country ID."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    var username = $"{country.CountryDialingCode}{mobileNumber}";

                    // Check if username already exists
                    // Note: IsDeleted filter is now applied globally
                    var usernameExists = await db.Users
                        .AnyAsync(u => u.UserName == username, cancellationToken);

                    if (usernameExists)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "MobileNumber",
                            ErrorMessage = $"Username '{username}' already exists."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Lookup designation
                    string? designationId = null;
                    if (!string.IsNullOrWhiteSpace(designationTitle))
                    {
                        if (designations.TryGetValue(designationTitle.ToLower(), out var desId))
                            designationId = desId;
                        else
                        {
                            errors.Add(new ImportError
                            {
                                RowNumber = rowIndex + 1,
                                Field = "DesignationTitle",
                                ErrorMessage = $"Designation '{designationTitle}' not found."
                            });
                            importResult.FailureCount++;
                            await transaction.RollbackAsync(cancellationToken);
                            continue;
                        }
                    }

                    // Lookup branch
                    string? branchId = null;
                    if (!string.IsNullOrWhiteSpace(branchCode))
                    {
                        if (branches.TryGetValue(branchCode.ToLower(), out var brId))
                            branchId = brId;
                        else
                        {
                            errors.Add(new ImportError
                            {
                                RowNumber = rowIndex + 1,
                                Field = "BranchCode",
                                ErrorMessage = $"Branch Code '{branchCode}' not found."
                            });
                            importResult.FailureCount++;
                            await transaction.RollbackAsync(cancellationToken);
                            continue;
                        }
                    }

                    // Parse optional fields
                    int? permanentWard = null;
                    if (!string.IsNullOrWhiteSpace(permanentWardStr) && int.TryParse(permanentWardStr, out var pw))
                        permanentWard = pw;

                    int? temporaryWard = null;
                    if (!string.IsNullOrWhiteSpace(temporaryWardStr) && int.TryParse(temporaryWardStr, out var tw))
                        temporaryWard = tw;

                    bool isActive = true;
                    if (!string.IsNullOrWhiteSpace(isActiveStr))
                    {
                        if (bool.TryParse(isActiveStr, out var ia))
                            isActive = ia;
                        else if (isActiveStr.Equals("1") || isActiveStr.Equals("yes", StringComparison.OrdinalIgnoreCase))
                            isActive = true;
                        else if (isActiveStr.Equals("0") || isActiveStr.Equals("no", StringComparison.OrdinalIgnoreCase))
                            isActive = false;
                    }

                    // Generate default password if not provided
                    if (string.IsNullOrWhiteSpace(password))
                        password = "DefaultPassword123!"; // You might want to generate a random password

                    // Create ApplicationUser
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = username,
                        Email = email.Trim(),
                        PhoneNumber = mobileNumber,
                        PhoneCountryId = countryId,
                        LockoutEnabled = true,
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TenantId = tenantId
                    };

                    var createUserResult = await userManager.CreateAsync(user, password);
                    if (!createUserResult.Succeeded)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "User",
                            ErrorMessage = createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Add FoDo role
                    var roleResult = await userManager.AddToRoleAsync(user, SystemRoles.FoDo);
                    if (!roleResult.Succeeded)
                    {
                        errors.Add(new ImportError
                        {
                            RowNumber = rowIndex + 1,
                            Field = "Role",
                            ErrorMessage = roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign role."
                        });
                        importResult.FailureCount++;
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    // Create Fodo entity
                    var fodo = new Fodo
                    {
                        Id = Guid.NewGuid().ToString(),
                        FullName = fullName.Trim(),
                        EmployeeId = employeeId.Trim(),
                        UserId = user.Id,
                        DesignationId = designationId,
                        BranchId = branchId,
                        PermanentProvince = string.IsNullOrWhiteSpace(permanentProvince) ? null : permanentProvince.Trim(),
                        PermanentDistrict = string.IsNullOrWhiteSpace(permanentDistrict) ? null : permanentDistrict.Trim(),
                        PermanentMunicipality = string.IsNullOrWhiteSpace(permanentMunicipality) ? null : permanentMunicipality.Trim(),
                        PermanentWard = permanentWard,
                        TemporaryProvince = string.IsNullOrWhiteSpace(temporaryProvince) ? null : temporaryProvince.Trim(),
                        TemporaryDistrict = string.IsNullOrWhiteSpace(temporaryDistrict) ? null : temporaryDistrict.Trim(),
                        TemporaryMunicipality = string.IsNullOrWhiteSpace(temporaryMunicipality) ? null : temporaryMunicipality.Trim(),
                        TemporaryWard = temporaryWard,
                        IsActive = isActive,
                        TenantId = tenantId
                    };

                    await db.Fodos.AddAsync(fodo, cancellationToken);
                    await db.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    importResult.SuccessCount++;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "General",
                        ErrorMessage = $"Error processing row: {ex.Message}"
                    });
                    importResult.FailureCount++;
                }
            }

            importResult.TotalRows = sheet.LastRowNum;
            importResult.Errors = errors;

            return Result<ImportResult>.Success(importResult);
        }
        catch (Exception ex)
        {
            return Result<ImportResult>.Failed($"Error importing Excel file: {ex.Message}");
        }
    }

    private string? GetCellValue(IRow row, Dictionary<string, int> columnMap, params string[] columnNames)
    {
        foreach (var columnName in columnNames)
        {
            if (columnMap.TryGetValue(columnName.ToLower(), out var index))
            {
                var cell = row.GetCell(index);
                if (cell != null)
                {
                    if (cell.CellType == CellType.String)
                        return cell.StringCellValue?.Trim();
                    if (cell.CellType == CellType.Numeric)
                        return cell.NumericCellValue.ToString();
                    if (cell.CellType == CellType.Boolean)
                        return cell.BooleanCellValue.ToString();
                }
            }
        }
        return null;
    }
}

