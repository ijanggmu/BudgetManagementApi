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
    public async Task<Result<List<AdminResponseDto>>> GetAdminsForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        // We only need to check if user is SuperAdmin for query filtering logic

        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var tenantId = requestModel.TenantId;

        IQueryable<Admin> query = db.Admins
            .Include(a => a.User);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        // For SuperAdmin: filter by tenantId if provided, otherwise show all
        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                // TenantId on Admin can be null for older/legacy records; fall back to role tenant assignment.
                // If the user's roles include any tenant-specific role for the requested tenant, include them.
                var tenantUserIds = await (from ur in db.UserRoles.IgnoreQueryFilters()
                                            join r in db.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                                            where ur.UserId != null
                                                  && !ur.IsDeleted
                                                  && !r.IsDeleted
                                                  && r.TenantId == tenantId
                                            select ur.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                query = query.Where(a => a.TenantId == tenantId || tenantUserIds.Contains(a.UserId));
            }
            // If tenantId is null, show all admins (global query filter will be ignored)
            query = query.IgnoreQueryFilters();
        }
        // SuperAdmin uses IgnoreQueryFilters() for cross-tenant listing; re-apply soft-delete exclusion
        // so removed admins do not reappear (global IsDeleted filter was bypassed).
        if (isSuperAdmin)
            query = query.Where(a => !a.IsDeleted && !a.User.IsDeleted);

        // For Admin: only show admins from their tenant (global query filter applies)
        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);

        var admins = await result.ToListAsync(cancellationToken);

        // Resolve tenant names for all unique tenant IDs.
        // If Admin.TenantId is missing, resolve it from the tenant-specific role(s) assigned to the user.
        var adminsMissingTenantId = admins
            .Where(a => string.IsNullOrWhiteSpace(a.TenantId))
            .Select(a => a.UserId)
            .Distinct()
            .ToList();

        var resolvedTenantIdByUserId = new Dictionary<string, string>();
        if (adminsMissingTenantId.Count > 0)
        {
            var resolved = await (from ur in db.UserRoles.IgnoreQueryFilters()
                                   join r in db.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                                   where adminsMissingTenantId.Contains(ur.UserId)
                                         && !ur.IsDeleted
                                         && !r.IsDeleted
                                         && r.TenantId != null
                                         && r.TenantId != string.Empty
                                   select new { ur.UserId, r.TenantId })
                .ToListAsync(cancellationToken);

            // If a user has multiple tenant-scoped roles, pick the first tenantId.
            resolvedTenantIdByUserId = resolved
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.First().TenantId);
        }

        var tenantIds = admins
            .Where(a => !string.IsNullOrWhiteSpace(a.TenantId))
            .Select(a => a.TenantId)
            .Union(resolvedTenantIdByUserId.Values)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var tenants = await db.Tenants
            .Where(t => tenantIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var departmentIds = admins
            .Select(a => a.User.DepartmentId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();
        var departmentNames = new Dictionary<string, string>(StringComparer.Ordinal);
        if (departmentIds.Count > 0)
        {
            IQueryable<Data.Entities.Tenant.Department> deptQuery = db.Departments.Where(d => departmentIds.Contains(d.Id));
            if (isSuperAdmin)
                deptQuery = deptQuery.IgnoreQueryFilters().Where(d => !d.IsDeleted);
            departmentNames = await deptQuery
                .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        }

        var dtos = new List<AdminResponseDto>();
        var adminRolesList = new List<(Data.Entities.AdminEntity.Admin Admin, List<string> Roles)>();
        foreach (var admin in admins)
        {
            var userRoles = (await userManager.GetRolesAsync(admin.User)).ToList();
            adminRolesList.Add((admin, userRoles));
        }

        var roleDisplayMap = await GetRoleDisplayNameMapAsync(
            adminRolesList.SelectMany(x => x.Roles),
            cancellationToken);

        foreach (var (admin, userRoles) in adminRolesList)
        {
            var effectiveTenantId = !string.IsNullOrWhiteSpace(admin.TenantId)
                ? admin.TenantId
                : (resolvedTenantIdByUserId.TryGetValue(admin.UserId, out var tid) ? tid : null);

            var tenantName = !string.IsNullOrWhiteSpace(effectiveTenantId) &&
                              tenants.TryGetValue(effectiveTenantId, out string value)
                ? value
                : string.Empty;

            var deptId = string.IsNullOrWhiteSpace(admin.User.DepartmentId) ? null : admin.User.DepartmentId;
            var deptName = deptId != null && departmentNames.TryGetValue(deptId, out var dn) ? dn : null;

            dtos.Add(new AdminResponseDto(
                admin.Id,
                admin.FullName,
                admin.User.Email ?? string.Empty,
                admin.User.PhoneNumber,
                admin.User.UserName ?? string.Empty,
                admin.UserId,
                string.IsNullOrWhiteSpace(effectiveTenantId) ? null : effectiveTenantId,
                tenantName,
                [.. userRoles],
                MapRoleDisplayNames(userRoles, roleDisplayMap),
                admin.User.IsDisabled,
                admin.User.EmailConfirmed,
                admin.CreatedOn,
                deptId,
                deptName
            ));
        }
        var pagination = new Pagination
        {
            TotalPages = totalPage,
            CurrentPage = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
            TotalItems = totalCount,
        };

        return Result<List<AdminResponseDto>>.Success(dtos, pagination);
    }

    public async Task<Result<AdminResponseDto>> GetAdminByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var query = db.Admins
            .Include(a => a.User)
            .Where(a => a.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(a => !a.IsDeleted && !a.User.IsDeleted);

        var admin = await query.FirstOrDefaultAsync(cancellationToken);

        if (admin == null)
            return Result<AdminResponseDto>.Failed("Admin not found.");

        var userRoles = await userManager.GetRolesAsync(admin.User);
        var effectiveTenantId = !string.IsNullOrWhiteSpace(admin.TenantId)
            ? admin.TenantId
            : await (from ur in db.UserRoles.IgnoreQueryFilters()
                      join r in db.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                      where ur.UserId == admin.UserId
                            && !ur.IsDeleted
                            && !r.IsDeleted
                            && r.TenantId != null
                            && r.TenantId != string.Empty
                      select r.TenantId)
                .FirstOrDefaultAsync(cancellationToken);

        var tenantName = string.Empty;
        if (!string.IsNullOrWhiteSpace(effectiveTenantId))
        {
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == effectiveTenantId, cancellationToken);
            tenantName = tenant?.Name ?? string.Empty;
        }

        string? departmentName = null;
        var deptIdForGet = string.IsNullOrWhiteSpace(admin.User.DepartmentId) ? null : admin.User.DepartmentId;
        if (deptIdForGet != null)
        {
            IQueryable<Data.Entities.Tenant.Department> deptQuery = db.Departments.Where(d => d.Id == deptIdForGet);
            if (isSuperAdmin)
                deptQuery = deptQuery.IgnoreQueryFilters().Where(d => !d.IsDeleted);
            departmentName = await deptQuery.Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        }

        var roleDisplayMap = await GetRoleDisplayNameMapAsync(userRoles, cancellationToken);

        var dto = new AdminResponseDto(
            admin.Id,
            admin.FullName,
            admin.User.Email ?? string.Empty,
            admin.User.PhoneNumber,
            admin.User.UserName ?? string.Empty,
            admin.UserId,
            string.IsNullOrWhiteSpace(effectiveTenantId) ? null : effectiveTenantId,
            tenantName,
            [.. userRoles],
            MapRoleDisplayNames(userRoles, roleDisplayMap),
            admin.User.IsDisabled,
            admin.User.EmailConfirmed,
            admin.CreatedOn,
            deptIdForGet,
            departmentName
        );

        return Result<AdminResponseDto>.Success(dto);
    }

    public async Task<Result<AdminResponseDto>> CreateAsync(CreateAdminDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var isSuperAdmin = userProfileService.IsSuperAdmin();

            // Get tenant ID from current user (for Admin) or from request body (for SuperAdmin)
            var userId = userProfileService.GetUserId();
            var user = await userManager.FindByIdAsync(userId);
            var tenantId = isSuperAdmin
                ? (!string.IsNullOrWhiteSpace(dto.TenantId) ? dto.TenantId : user?.TenantId)
                : db.CurrentTenantId;

            var normalizedUserName = userManager.NormalizeName(dto.Username);
            if (string.IsNullOrEmpty(normalizedUserName))
                return Result<AdminResponseDto>.Failed("Username is invalid.");

            var usernameTaken = await db.Users
                .AnyAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);

            if (usernameTaken)
                return Result<AdminResponseDto>.Failed("Username already exists.");

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var normalizedEmail = userManager.NormalizeEmail(dto.Email);
                var emailExists = await db.Users
                    .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

                if (emailExists)
                    return Result<AdminResponseDto>.Failed("Email already exists.");
            }

            var tenantAssignableTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                SystemRoles.Admin,
                SystemRoles.CEO,
                SystemRoles.CFO,
                SystemRoles.HOD,
                SystemRoles.HodAssistance
            };

            List<string> rolesToAssign;
            if (dto.Roles != null && dto.Roles.Count != 0)
            {
                rolesToAssign = dto.Roles;
                if (!isSuperAdmin && dto.Roles.Any(r => string.Equals(r, SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase)))
                    return Result<AdminResponseDto>.Failed("You cannot assign SuperAdmin role.");
            }
            else if (!isSuperAdmin && !string.IsNullOrEmpty(tenantId))
            {
                var tenantAdminRoleName = await db.Roles.IgnoreQueryFilters()
                    .Where(r => r.TenantId == tenantId && r.RoleType == SystemRoles.Admin && !r.IsDeleted)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                rolesToAssign = string.IsNullOrEmpty(tenantAdminRoleName)
                    ? new List<string> { SystemRoles.Admin }
                    : new List<string> { tenantAdminRoleName };
            }
            else
            {
                rolesToAssign = new List<string> { SystemRoles.Admin };
            }

            var distinctRoleNames = rolesToAssign.Distinct(StringComparer.Ordinal).ToList();

            List<ApplicationRole> resolvedRoles;
            if (isSuperAdmin)
            {
                resolvedRoles = await db.Roles.IgnoreQueryFilters()
                    .Where(r => distinctRoleNames.Contains(r.Name) && !r.IsDeleted)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                resolvedRoles = await db.Roles
                    .Where(r => distinctRoleNames.Contains(r.Name) && !r.IsDeleted)
                    .ToListAsync(cancellationToken);
            }

            if (resolvedRoles.Count != distinctRoleNames.Count)
                return Result<AdminResponseDto>.Failed("One or more roles are invalid.");

            if (!isSuperAdmin && !string.IsNullOrEmpty(tenantId))
            {
                foreach (var r in resolvedRoles)
                {
                    if (r.TenantId != tenantId)
                        return Result<AdminResponseDto>.Failed("Role does not belong to this tenant.");
                    if (!tenantAssignableTypes.Contains(r.RoleType ?? string.Empty))
                        return Result<AdminResponseDto>.Failed(
                            "You may only assign predefined tenant roles (Tenant Admin, CEO, CFO, HOD, HOD Assistant).");
                }
            }

            var needsDepartment = resolvedRoles.Any(r =>
                string.Equals(r.RoleType, SystemRoles.HOD, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleType, SystemRoles.HodAssistance, StringComparison.OrdinalIgnoreCase));

            if (needsDepartment)
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return Result<AdminResponseDto>.Failed("Tenant is required when assigning Head of Department or HOD Assistant.");
                if (string.IsNullOrWhiteSpace(dto.DepartmentId))
                    return Result<AdminResponseDto>.Failed("Department is required for Head of Department and HOD Assistant.");
                var departmentValid = await db.Departments.AnyAsync(
                    d => d.Id == dto.DepartmentId && d.TenantId == tenantId && !d.IsDeleted,
                    cancellationToken);
                if (!departmentValid)
                    return Result<AdminResponseDto>.Failed("Department not found or does not belong to this tenant.");
            }

            var departmentIdToSet = needsDepartment ? dto.DepartmentId : null;

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
                TenantId = tenantId,
                DepartmentId = departmentIdToSet
            };

            var createUserResult = await userManager.CreateAsync(adminUser, dto.Password);
            if (!createUserResult.Succeeded)
                return Result<AdminResponseDto>.Failed(createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user.");

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

            string? createdDeptName = null;
            if (!string.IsNullOrWhiteSpace(adminUser.DepartmentId))
            {
                createdDeptName = await db.Departments
                    .Where(d => d.Id == adminUser.DepartmentId)
                    .Select(d => d.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var roleDisplayMap = await GetRoleDisplayNameMapAsync(userRoles, cancellationToken);

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
                MapRoleDisplayNames(userRoles, roleDisplayMap),
                adminUser.IsDisabled,
                adminUser.EmailConfirmed,
                admin.CreatedOn,
                string.IsNullOrWhiteSpace(adminUser.DepartmentId) ? null : adminUser.DepartmentId,
                createdDeptName
            );

            return Result<AdminResponseDto>.Success(responseDto);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            var inner = ex.InnerException?.Message ?? ex.Message;
            if (inner.Contains("23505", StringComparison.Ordinal) ||
                inner.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                inner.Contains("UserNameIndex", StringComparison.OrdinalIgnoreCase))
                return Result<AdminResponseDto>.Failed("Username or email is already in use by an active account.");
            return Result<AdminResponseDto>.Failed($"Could not save the user: {inner}");
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
            var isSuperAdmin = userProfileService.IsSuperAdmin();

            var query = db.Admins
                .Include(a => a.User)
                .Where(a => a.Id == id);
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters().Where(a => !a.IsDeleted && !a.User.IsDeleted);

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

            admin.User.IsDisabled = dto.IsDisabled;

            // Update roles if provided
            if (dto.Roles != null)
            {
                var tenantAssignableTypesUpdate = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    SystemRoles.Admin,
                    SystemRoles.CEO,
                    SystemRoles.CFO,
                    SystemRoles.HOD,
                    SystemRoles.HodAssistance
                };

                if (!isSuperAdmin && dto.Roles.Any(r => string.Equals(r, SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase)))
                    return Result<AdminResponseDto>.Failed("You cannot assign SuperAdmin role.");

                var distinctUpdateRoleNames = dto.Roles.Distinct(StringComparer.Ordinal).ToList();

                List<ApplicationRole> resolvedUpdateRoles;
                if (isSuperAdmin)
                {
                    resolvedUpdateRoles = await db.Roles.IgnoreQueryFilters()
                        .Where(r => distinctUpdateRoleNames.Contains(r.Name) && !r.IsDeleted)
                        .ToListAsync(cancellationToken);
                }
                else
                {
                    resolvedUpdateRoles = await db.Roles
                        .Where(r => distinctUpdateRoleNames.Contains(r.Name) && !r.IsDeleted)
                        .ToListAsync(cancellationToken);
                }

                if (resolvedUpdateRoles.Count != distinctUpdateRoleNames.Count)
                    return Result<AdminResponseDto>.Failed("One or more roles are invalid.");

                var adminTenantId = admin.TenantId ?? db.CurrentTenantId;
                if (!isSuperAdmin && !string.IsNullOrEmpty(adminTenantId))
                {
                    foreach (var r in resolvedUpdateRoles)
                    {
                        if (r.TenantId != adminTenantId)
                            return Result<AdminResponseDto>.Failed("Role does not belong to this tenant.");
                        if (!tenantAssignableTypesUpdate.Contains(r.RoleType ?? string.Empty))
                            return Result<AdminResponseDto>.Failed(
                                "You may only assign predefined tenant roles (Tenant Admin, CEO, CFO, HOD, HOD Assistant).");
                    }
                }

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

            if (dto.DepartmentId != null)
            {
                admin.User.DepartmentId = string.IsNullOrWhiteSpace(dto.DepartmentId)
                    ? null
                    : dto.DepartmentId.Trim();
            }

            var effectiveTenantIdForDept = admin.TenantId ?? db.CurrentTenantId;
            var roleNamesAfterUpdate = await userManager.GetRolesAsync(admin.User);
            var distinctAfter = roleNamesAfterUpdate.Distinct(StringComparer.Ordinal).ToList();
            List<ApplicationRole> resolvedAfterUpdate;
            if (isSuperAdmin)
            {
                resolvedAfterUpdate = await db.Roles.IgnoreQueryFilters()
                    .Where(r => distinctAfter.Contains(r.Name) && !r.IsDeleted)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                resolvedAfterUpdate = await db.Roles
                    .Where(r => distinctAfter.Contains(r.Name) && !r.IsDeleted)
                    .ToListAsync(cancellationToken);
            }

            var needsDepartmentAfter = resolvedAfterUpdate.Any(r =>
                string.Equals(r.RoleType, SystemRoles.HOD, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleType, SystemRoles.HodAssistance, StringComparison.OrdinalIgnoreCase));

            if (needsDepartmentAfter)
            {
                if (string.IsNullOrWhiteSpace(effectiveTenantIdForDept))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<AdminResponseDto>.Failed("Cannot validate department: tenant is unknown.");
                }

                if (string.IsNullOrWhiteSpace(admin.User.DepartmentId))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<AdminResponseDto>.Failed("Department is required for Head of Department and HOD Assistant.");
                }

                var departmentOk = await db.Departments.AnyAsync(
                    d => d.Id == admin.User.DepartmentId && d.TenantId == effectiveTenantIdForDept && !d.IsDeleted,
                    cancellationToken);
                if (!departmentOk)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<AdminResponseDto>.Failed("Department not found or does not belong to this tenant.");
                }
            }
            else
            {
                admin.User.DepartmentId = null;
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

            string? updatedDeptName = null;
            if (!string.IsNullOrWhiteSpace(admin.User.DepartmentId))
            {
                updatedDeptName = await db.Departments
                    .Where(d => d.Id == admin.User.DepartmentId)
                    .Select(d => d.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var roleDisplayMap = await GetRoleDisplayNameMapAsync(userRoles, cancellationToken);

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
                MapRoleDisplayNames(userRoles, roleDisplayMap),
                admin.User.IsDisabled,
                admin.User.EmailConfirmed,
                admin.CreatedOn,
                string.IsNullOrWhiteSpace(admin.User.DepartmentId) ? null : admin.User.DepartmentId,
                updatedDeptName
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
            var currentUserId = userProfileService.GetUserId();
            var isSuperAdmin = userProfileService.IsSuperAdmin();

            // Ignore tenant query filters: Admin.TenantId is often null for legacy rows while User still belongs
            // to the current tenant (resolved via roles). The global Admin filter would hide those rows and break delete.
            var admin = await db.Admins
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

            if (admin == null)
                return Result<bool>.Failed("Admin not found.");

            var user = await db.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == admin.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
                return Result<bool>.Failed("User account not found or already removed.");

            if (!isSuperAdmin)
            {
                var currentTenantId = db.CurrentTenantId;
                if (string.IsNullOrWhiteSpace(currentTenantId))
                    return Result<bool>.Failed("Tenant context is missing. Cannot delete this user.");

                var effectiveTenantId = await GetEffectiveTenantIdForAdminAsync(admin, cancellationToken);
                if (string.IsNullOrWhiteSpace(effectiveTenantId) ||
                    !string.Equals(effectiveTenantId, currentTenantId, StringComparison.Ordinal))
                    return Result<bool>.Failed("You do not have permission to delete this user.");
            }

            if (string.Equals(admin.UserId, currentUserId, StringComparison.Ordinal))
                return Result<bool>.Failed("You cannot delete your own account.");

            admin.IsDeleted = true;
            user.IsDeleted = true;

            db.Admins.Update(admin);
            db.Users.Update(user);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failed($"Could not delete the user: {ex.Message}");
        }
    }

    /// <summary>Resolves tenant for an admin row when <see cref="Admin.TenantId"/> is not set (legacy / role-only linkage).</summary>
    private async Task<Dictionary<string, string>> GetRoleDisplayNameMapAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken)
    {
        var names = roleNames.Distinct(StringComparer.Ordinal).Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
        if (names.Count == 0)
            return new Dictionary<string, string>(StringComparer.Ordinal);

        var pairs = await db.Roles.IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.Name != null && names.Contains(r.Name))
            .Select(r => new { Name = r.Name!, Label = r.RoleDisplayName ?? r.Name! })
            .ToListAsync(cancellationToken);

        return pairs.ToDictionary(x => x.Name, x => x.Label, StringComparer.Ordinal);
    }

    private static List<string> MapRoleDisplayNames(
        IEnumerable<string> roleNames,
        IReadOnlyDictionary<string, string> displayMap)
        => roleNames.Select(name => displayMap.TryGetValue(name, out var label) ? label : name).ToList();

    private async Task<string?> GetEffectiveTenantIdForAdminAsync(Admin admin, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(admin.TenantId))
            return admin.TenantId;

        return await (from ur in db.UserRoles.IgnoreQueryFilters()
                join r in db.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                where ur.UserId == admin.UserId
                      && !ur.IsDeleted
                      && !r.IsDeleted
                      && r.TenantId != null
                      && r.TenantId != string.Empty
                select r.TenantId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            // Prefer explicit TenantId on the request body; fall back to Sieve filter for older clients
            if (string.IsNullOrWhiteSpace(requestModel.TenantId)
                && !string.IsNullOrEmpty(requestModel.Filters)
                && requestModel.Filters.Contains("TenantId==", StringComparison.Ordinal))
            {
                var tenantIdMatch = System.Text.RegularExpressions.Regex.Match(requestModel.Filters, @"TenantId==([^,|]+)");
                if (tenantIdMatch.Success)
                    requestModel.TenantId = tenantIdMatch.Groups[1].Value.Trim();
            }

            var result = await GetAdminsForAdminAsync(requestModel, cancellationToken);
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
                { "DepartmentId", "Department ID" },
                { "DepartmentName", "Department Name" },
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

