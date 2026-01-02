using System.Linq.Expressions;
using System.Threading;
using Data.Context;
using Data.Entities.Identity;
using Data.Infrastructure;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Role;

public class RoleService(
       RoleManager<ApplicationRole> roleManager,
       ApplicationDataContext dataContext,
       ISieveExtension sieveExtenstion,
       ITenantContext tenantContext
           ) : IRoleService
{
    public Result<List<string>> GetAllSystemRoles()
    {
        return Result<List<string>>.Success(SystemRoles.GetAllDefaultRolesExceptSuperAdmin());
    }

    public async Task<Result<List<string>>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default)
    {
        var query = roleManager.Roles
            .AsNoTracking()
            .Where(a => !a.IsDeleted);
        
        // Filter by tenant for non-superadmin admins (tenant-wise roles)
        // SuperAdmin can see all roles, tenant admins see their tenant's roles + global roles (TenantId == null)
        if (!string.IsNullOrWhiteSpace(tenantContext?.TenantId))
        {
            query = query.Where(a => a.TenantId == tenantContext.TenantId || a.TenantId == null);
        }
        
        var roleNames = await query
            .OrderByDescending(x => x.CreatedOn)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);
            
        return Result<List<string>>.Success(roleNames);
    }
    public async Task<Result<List<RoleResponseModel>>> GetAllRolesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        Expression<Func<ApplicationRole, bool>> predicate = c => !c.IsDeleted;
        
        // Filter by tenant for non-superadmin admins (tenant-wise roles)
        // SuperAdmin can see all roles, tenant admins see their tenant's roles + global roles (TenantId == null)
        if (!string.IsNullOrWhiteSpace(tenantContext?.TenantId))
        {
            predicate = c => !c.IsDeleted && (c.TenantId == tenantContext.TenantId || c.TenantId == null);
        }

        var query = roleManager.Roles
                                 .Where(predicate)
                                 .AsNoTracking();

        var (result, totalCount, totalPage) = await sieveExtenstion.ApplySieve(query, requestModel);

        var Admins = await result.Select(x => new RoleResponseModel
        {
            RoleId = x.Id,
            RoleName = x.Name,
            RoleDescription = x.Description,
            RoleType = x.RoleType,
            TotalUserAssignedWithRole = dataContext.UserRoles.Where(y => y.RoleId == x.Id).Count()
        }).ToListAsync(cancellationToken);


        var pagination = new Pagination
        {
            TotalPages = totalPage,
            CurrentPage = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
            TotalItems = totalCount,
        };
        return Result<List<RoleResponseModel>>.Success(Admins, pagination);
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

        // Check if role name exists in the same tenant (or globally if TenantId is null)
        var checkIfRoleNameExists = await dataContext.Roles
            .AnyAsync(r => r.Name.ToLower() == roleNameLower 
                && !r.IsDeleted 
                && (r.TenantId == tenantContext.TenantId || r.TenantId == null), cancellationToken);
        if (checkIfRoleNameExists)
            return Result<MessageResponseModel>.Failed("Role already exists in this tenant.");

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = model.RoleName,
            RoleType = model.RoleType,
            Description = model.RoleDescription,
            // Set TenantId for tenant-wise role creation (non-superadmin admins)
            TenantId = tenantContext?.TenantId
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
        
        // Check if role name exists in the same tenant (or globally if TenantId is null)
        var checkIfRoleNameExists = await roleManager.Roles
            .AnyAsync(r => (r.Name.ToLower() == roleModel.RoleName.Trim().ToLower() ||
                           r.Name.ToLower() == modelNameWhiteSpaceRemoved.ToLower())
                           && !r.IsDeleted
                           && r.Id != roleModel.RoleId
                           && (r.TenantId == role.TenantId || (r.TenantId == null && role.TenantId == null)), cancellationToken);
        var nameNotChanged = role.Name.ToLower() == roleModel.RoleName.ToLower();

        if (role != null)
        {
            var notEditableRoles = SystemRoles.GetNotDeletableRoles();

            if (notEditableRoles.Any(x => x == role.Name))
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
        var transaction = await dataContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var role = await dataContext.Roles.FindAsync(new object[] { roleId }, cancellationToken);

            if (role == null)
                return Result<MessageResponseModel>.Failed("Invalid Role.");

            var notDeletableRoles = SystemRoles.GetNotDeletableRoles();

            if (notDeletableRoles.Any(x => x == role.Name))
            {
                return Result<MessageResponseModel>.Failed("Role cannot be deleted.");
            }

            var userexists = await dataContext.UserRoles.AnyAsync(x => x.RoleId == role.Id, cancellationToken);
            if (userexists)
                return Result<MessageResponseModel>.Failed("Role is assigned to a user.");

            var roleClaims = await dataContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync(cancellationToken);

            dataContext.RoleClaims.RemoveRange(roleClaims);
            await dataContext.SaveChangesAsync(cancellationToken);

            await roleManager.DeleteAsync(role);

            await transaction.CommitAsync(cancellationToken);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role deleted successfully."));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
