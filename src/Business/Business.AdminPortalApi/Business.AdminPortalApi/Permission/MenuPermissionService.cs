using System.Collections.Immutable;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Common.Menu;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Constant.Permission;
using SharedKernel.Operation;
using static SharedKernel.Constant.Permission.MenuPermissionsList;

namespace Business.AdminPortalApi.Permission;
public class MenuPermissionService : IMenuPermissionService
{
    private readonly ApplicationDataContext _context;
    private readonly IUserProfileService _personAccessor;

    public MenuPermissionService(ApplicationDataContext context,
        IUserProfileService personAccessor)
    {
        _context = context;
        _personAccessor = personAccessor;
    }

    public Result<MenuModel> GetMenu()
    {
        var roleId = _personAccessor.GetRoleId();

        if (string.IsNullOrEmpty(roleId))
            return Result<MenuModel>.Success(new MenuModel());

        var roleNameList = roleId.Split(",");

        var permissionList = (from role in _context.Roles
                              join roleClaim in _context.RoleClaims
                              on role.Id equals roleClaim.RoleId
                              where roleNameList.Contains(role.Name) && !role.IsDeleted
                              select roleClaim.Permissions)
                          .ToList();

        var permissions = permissionList.SelectMany(x => x).ToList();

        if (!permissions.Any())
            return Result<MenuModel>.Success(new MenuModel());

        var menus = MenuManager.GetMenusForPermissions(permissions);

        return Result<MenuModel>.Success(new MenuModel { MenuList = menus });

    }

    public Result<RolePermissionViewModel> GetAllMenuByRoleId(string roleId)
    {
        var existingRole = _context.Roles.Where(x => x.Id == roleId).Select(y => new
        {
            y.Id,
            y.RoleType,
            y.Name
        }).FirstOrDefault();

        if (existingRole == null)
            return Result<RolePermissionViewModel>.Failed("Role not found.");

        RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel
        {
            RoleId = existingRole.Id,
            RoleName = existingRole.Name,
            RolePermissionGroup = new List<RolePermissionGroup>()
        };

        var existingPermissions = _context.RoleClaims
                                                 .Where(q => q.RoleId == roleId)
                                                 .Select(x => x.Permissions)
                                                 .FirstOrDefault();

        var groupedPermissions = MenuPermissionsList._list;

        foreach (var menuItem in groupedPermissions)
        {
            RolePermissionGroup rolePermissionGroup = rolePermissionViewModel.RolePermissionGroup
                .FirstOrDefault(group => group.Module == menuItem.MenuName);

            if (rolePermissionGroup == null)
            {
                rolePermissionGroup = new RolePermissionGroup
                {
                    Module = menuItem.MenuName,
                    Rank = menuItem.Rank,
                    HideChildren = menuItem.HideChildren
                };
                rolePermissionViewModel.RolePermissionGroup.Add(rolePermissionGroup);
            }

            AddPermissionsWithChildren(menuItem, existingPermissions, rolePermissionGroup);
        }

        return Result<RolePermissionViewModel>.Success(rolePermissionViewModel);


    }

    private void AddPermissionsWithChildren(MenuItem menuItem, List<string> permissions, RolePermissionGroup rolePermissionGroup)
    {
        foreach (var permission in menuItem.Permissions)
        {
            PermissionList permissionList = new PermissionList
            {
                PermissionId = permission.Value,
                PermissionTitle = permission.Title,
                IsAutoCheck = permission.IsAutoCheck,
                HasClaim = (permissions != null) && permissions.Contains(permission.Value)
            };
            rolePermissionGroup.Permissions.Add(permissionList);
        }

        if (menuItem.Children != null)
        {
            foreach (var child in menuItem.Children)
            {
                RolePermissionGroup childRolePermissionGroup = new RolePermissionGroup
                {
                    Module = child.MenuName,
                    Rank = child.Rank,
                    HideChildren = child.HideChildren
                };
                rolePermissionGroup.Childrens.Add(childRolePermissionGroup);
                AddPermissionsWithChildren(child, permissions, childRolePermissionGroup);
            }
        }
    }

    public async Task<Result<MessageResponseModel>> AssignRolePermissionAsync(PermissionManagementViewModel requestModel)
    {
        var roleId = requestModel.RoleId;

        var hasRole = await _context.Roles.AnyAsync(x => x.Id == roleId);
        if (!hasRole)
            return Result<MessageResponseModel>.Failed("Role not found.");

        var roleClaim = await _context.RoleClaims.Where(x => x.RoleId == roleId).FirstOrDefaultAsync();

        if (roleClaim == null)
        {
            _context.RoleClaims.Add(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = requestModel.ClaimList
            });
        }
        else
        {
            roleClaim.Permissions = requestModel.ClaimList;
            _context.RoleClaims.Update(roleClaim);
        }

        await _context.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Permissions of role added successfully!"));
    }
}
