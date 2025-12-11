using System.Linq.Expressions;
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
public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDataContext _context;
    private readonly ISieveExtension _sieveExtension;
    public RoleService(
           RoleManager<ApplicationRole> roleManager,
           ApplicationDataContext dataContext,
           ISieveExtension sieveExtenstion
           )
    {
        _roleManager = roleManager;
        _context = dataContext;
        _sieveExtension = sieveExtenstion;
    }

    public Result<List<string>> GetAllSystemRoles()
    {
        return Result<List<string>>.Success(SystemRoles.GetAllDefaultRolesExceptSuperAdmin());
    }

    public async Task<Result<List<string>>> GetAllRoleNamesAsync() => Result<List<string>>.Success(await _roleManager.Roles
                                                                    .AsNoTracking()
                                                                    .Where(a => !a.IsDeleted)
                                                                    .OrderByDescending(x => x.CreatedOn)
                                                                    .Select(x => x.Name)
                                                                    .ToListAsync());
    public async Task<Result<List<RoleResponseModel>>> GetAllRolesAsync(CommonPaginationRequestModel requestModel)
    {
        Expression<Func<ApplicationRole, bool>> predicate = c => !c.IsDeleted;

        var query = _roleManager.Roles
                                 .Where(predicate)
                                 .AsNoTracking();

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);

        var customers = await result.Select(x => new RoleResponseModel
        {
            RoleId = x.Id,
            RoleName = x.Name,
            RoleDescription = x.Description,
            RoleType = x.RoleType,
            TotalUserAssignedWithRole = _context.UserRoles.Where(y => y.RoleId == x.Id).Count()
        }).ToListAsync();


        var pagination = new Pagination
        {
            TotalPages = totalPage,
            CurrentPage = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
            TotalItems = totalCount,
        };
        return Result<List<RoleResponseModel>>.Success(customers, pagination);
    }

    public async Task<Result<MessageResponseModel>> CreateRoleAsync(CreateRoleRequestModel model)
    {

        var roleNameLower = model.RoleName.Trim().ToLower();
        var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleNameLower && r.IsDeleted);

        if (existingRole != null)
        {
            existingRole.IsDeleted = false;
            existingRole.Description = model.RoleDescription;
            await _roleManager.UpdateAsync(existingRole);
            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role created successfully."));
        }

        var checkIfRoleNameExists = await _context.Roles.AnyAsync(r => r.Name.ToLower() == roleNameLower && !r.IsDeleted);
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
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role created successfully."));

        return Result<MessageResponseModel>.Failed(result.Errors.Select(x => x.Description).FirstOrDefault());


    }

    public async Task<Result<RoleResponseModel>> GetRoleByIdAsync(string roleId)
    {
        var role = await _roleManager.Roles.Where(x => x.Id == roleId && !x.IsDeleted).FirstOrDefaultAsync();
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

    public async Task<Result<MessageResponseModel>> UpdateRoleAsync(UpdateRoleRequestModel roleModel)
    {

        var role = await _roleManager.Roles.Where(x => x.Id == roleModel.RoleId).FirstOrDefaultAsync();
        var modelNameWhiteSpaceRemoved = string.Concat(roleModel.RoleName.Where(c => !char.IsWhiteSpace(c)));
        var checkIfRoleNameExists = await _roleManager.Roles
            .AnyAsync(r => r.Name.ToLower() == roleModel.RoleName.Trim().ToLower() ||
                           r.Name.ToLower() == modelNameWhiteSpaceRemoved.ToLower());
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

                await _roleManager.UpdateAsync(role);
                return Result<MessageResponseModel>.Success(new MessageResponseModel("Role updated successfully."));
            }
            return Result<MessageResponseModel>.Failed("Role name already exists.");
        }
        return Result<MessageResponseModel>.Failed("Role not found.");
    }


    public async Task<Result<MessageResponseModel>> DeleteRoleAsync(string roleId)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var role = await _context.Roles.FindAsync(roleId);

            if (role == null)
                return Result<MessageResponseModel>.Failed("Invalid Role.");

            var notDeletableRoles = SystemRoles.GetNotDeletableRoles();

            if (notDeletableRoles.Any(x => x == role.Name))
            {
                return Result<MessageResponseModel>.Failed("Role cannot be deleted.");
            }

            var userexists = await _context.UserRoles.AnyAsync(x => x.RoleId == role.Id);
            if (userexists)
                return Result<MessageResponseModel>.Failed("Role is assigned to a user.");

            var roleClaims = await _context.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();

            _context.RoleClaims.RemoveRange(roleClaims);
            await _context.SaveChangesAsync();

            await _roleManager.DeleteAsync(role);

            await transaction.CommitAsync();

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Role deleted successfully."));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
