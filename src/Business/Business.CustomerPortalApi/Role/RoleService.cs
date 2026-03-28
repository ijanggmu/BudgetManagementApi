using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Role;

public class RoleService(
       RoleManager<ApplicationRole> roleManager,
       ApplicationDataContext dataContext,
       ISieveExtension sieveExtenstion,
       IUserProfileService userProfileService
           ) : IRoleService
{
    public Result<List<string>> GetAllSystemRoles()
    {
        return Result<List<string>>.Success(SystemRoles.GetAllDefaultRolesExceptSuperAdmin());
    }

    public async Task<Result<List<string>>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default) => Result<List<string>>.Success(await roleManager.Roles
                                                                    .AsNoTracking()
                                                                    .Where(a => !a.IsDeleted)
                                                                    .OrderByDescending(x => x.CreatedOn)
                                                                    .Select(x => x.Name)
                                                                    .ToListAsync(cancellationToken));
    public async Task<Result<List<RoleResponseModel>>> GetAllRolesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        Expression<Func<ApplicationRole, bool>> predicate = c => !c.IsDeleted;

        // SuperAdmin: optional tenant scope from body (roles / user-management). Include global roles (TenantId null).
        if (isSuperAdmin && !string.IsNullOrWhiteSpace(requestModel.TenantId))
        {
            var tid = requestModel.TenantId;
            predicate = c => !c.IsDeleted && (c.TenantId == tid || c.TenantId == null);
        }

        IQueryable<ApplicationRole> query = dataContext.Roles.AsNoTracking();
        if (isSuperAdmin)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.Where(predicate);
        var (result, totalCount, totalPage) = await sieveExtenstion.ApplySieve(query, requestModel);

        var roles = await result.Select(x => new RoleResponseModel
        {
            CreatedOn = x.CreatedOn,
            RoleId = x.Id,
            RoleName = x.Name,
            RoleDisplayName = x.RoleDisplayName ?? x.Name,
            RoleDescription = x.Description,
            RoleType = x.RoleType,
            TenantId = x.TenantId,
            TotalUserAssignedWithRole = dataContext.UserRoles.Where(y => y.RoleId == x.Id).Count(),

        }).ToListAsync(cancellationToken);

        var tenantIds = roles.Where(a => !string.IsNullOrEmpty(a.TenantId)).Select(a => a.TenantId).Distinct().ToList();
        var tenants = await dataContext.Tenants.Where(t => tenantIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        foreach (var role in roles)
        {
            var tenantName = !string.IsNullOrEmpty(role.TenantId) && tenants.TryGetValue(role.TenantId, out string value)
                ? value : string.Empty;
            role.TenantName = tenantName;
        }
        var pagination = new Pagination
        {
            TotalPages = totalPage,
            CurrentPage = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
            TotalItems = totalCount,
        };
        return Result<List<RoleResponseModel>>.Success(roles, pagination);
    }

    public async Task<Result<MessageResponseModel>> CreateRoleAsync(CreateRoleRequestModel model, CancellationToken cancellationToken = default)
    {

        var roleNameLower = model.RoleName.Trim().ToLower();
        var existingRole = await dataContext.Roles.FirstOrDefaultAsync(r => r.Name == roleNameLower && r.IsDeleted, cancellationToken);

        if (existingRole != null)
        {
            existingRole.IsDeleted = false;
            existingRole.Description = model.RoleDescription;
            await roleManager.UpdateAsync(existingRole);
            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role created successfully."));
        }

        var checkIfRoleNameExists = await dataContext.Roles.AnyAsync(r => r.Name.ToLower() == roleNameLower && !r.IsDeleted, cancellationToken);
        if (checkIfRoleNameExists)
            return Result<MessageResponseModel>.Failed("Role already exists.");

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = model.RoleName,
            RoleType = model.RoleType,
            Description = model.RoleDescription,
        };

        //if (!role.IsValidRoleType())
        //    return OperationResult.Failed("Invalid role type.");

        //role.AssignRoleLevel();
        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role created successfully."));

        return Result<MessageResponseModel>.Failed(result.Errors.Select(x => x.Description).FirstOrDefault());


    }

    public async Task<Result<RoleResponseModel>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var role = await roleManager.Roles.Where(x => x.Id == roleId && !x.IsDeleted).FirstOrDefaultAsync(cancellationToken);
        if (role == null)
            return Result<RoleResponseModel>.Failed("Role not found.");

        return Result<RoleResponseModel>.Success(new RoleResponseModel()
        {
            RoleId = role.Id,
            RoleType = role.RoleType,
            RoleName = role.Name,
            RoleDescription = role.Description
        });
    }

    public async Task<Result<MessageResponseModel>> UpdateRoleAsync(UpdateRoleRequestModel roleModel, CancellationToken cancellationToken = default)
    {

        var role = await roleManager.Roles.Where(x => x.Id == roleModel.RoleId).FirstOrDefaultAsync(cancellationToken);
        var modelNameWhiteSpaceRemoved = string.Concat(roleModel.RoleName.Where(c => !char.IsWhiteSpace(c)));
        var checkIfRoleNameExists = await roleManager.Roles
            .AnyAsync(r => r.Name.ToLower() == roleModel.RoleName.Trim().ToLower() ||
                           r.Name.ToLower() == modelNameWhiteSpaceRemoved.ToLower(), cancellationToken);
        var nameNotChanged = role.Name.ToLower() == roleModel.RoleName.ToLower();

        if (role != null)
        {
            if (SystemRoles.GetNotDeletableRoleTypes().Contains(role.RoleType ?? string.Empty))
                return Result<MessageResponseModel>.Failed("Role cannot be edited.");

            if (!checkIfRoleNameExists || nameNotChanged)
            {
                role.Name = roleModel.RoleName.Trim();
                role.Description = roleModel.RoleDescription?.Trim();

                await roleManager.UpdateAsync(role);
                return Result<MessageResponseModel>.Success(new MessageResponseModel("Role updated successfully."));
            }
            return Result<MessageResponseModel>.Failed("Role name already exists.");
        }
        return Result<MessageResponseModel>.Failed("Role not found.");
    }


    public async Task<Result<MessageResponseModel>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dataContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var roleIdClaim = userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrWhiteSpace(roleIdClaim) &&
                               roleIdClaim.Contains(SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase);

            IQueryable<ApplicationRole> roleQuery = dataContext.Roles;
            if (isSuperAdmin)
                roleQuery = roleQuery.IgnoreQueryFilters();

            var role = await roleQuery.FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);

            if (role == null)
                return Result<MessageResponseModel>.Failed("Role not found. It may have already been deleted.");

            if (!isSuperAdmin)
            {
                var tenantId = dataContext.CurrentTenantId;
                if (string.IsNullOrWhiteSpace(tenantId))
                    return Result<MessageResponseModel>.Failed("Tenant context is missing. Cannot delete roles.");

                if (!string.Equals(role.TenantId, tenantId, StringComparison.Ordinal))
                {
                    return Result<MessageResponseModel>.Failed(
                        "You cannot delete this role. It belongs to another organization or is not a tenant role you manage.");
                }
            }

            if (SystemRoles.GetNotDeletableRoleTypes().Contains(role.RoleType ?? string.Empty))
            {
                var label = string.IsNullOrWhiteSpace(role.RoleDisplayName) ? role.Name : role.RoleDisplayName;
                return Result<MessageResponseModel>.Failed(
                    $"Cannot delete \"{label}\": it is a predefined system role (type: {role.RoleType}). " +
                    "These roles are required for budgeting and approvals and cannot be removed.");
            }

            var assignedCount = await dataContext.UserRoles.CountAsync(
                x => x.RoleId == role.Id && !x.IsDeleted, cancellationToken);
            if (assignedCount > 0)
            {
                return Result<MessageResponseModel>.Failed(
                    $"Cannot delete \"{role.Name}\": {assignedCount} user(s) still have this role. " +
                    "Remove the role from every user first, then try deleting again.");
            }

            var roleClaims = await dataContext.RoleClaims
                .IgnoreQueryFilters()
                .Where(x => x.RoleId == role.Id)
                .ToListAsync(cancellationToken);

            dataContext.RoleClaims.RemoveRange(roleClaims);
            await dataContext.SaveChangesAsync(cancellationToken);

            var deleteResult = await roleManager.DeleteAsync(role);
            if (!deleteResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<MessageResponseModel>.Failed(
                    string.Join(" ", deleteResult.Errors.Select(e => e.Description)));
            }

            await transaction.CommitAsync(cancellationToken);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role deleted successfully."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<MessageResponseModel>.Failed($"Could not delete the role: {ex.Message}");
        }
    }
}
