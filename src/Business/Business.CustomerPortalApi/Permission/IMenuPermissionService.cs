using System.Threading;
using Models.Common;
using Models.Common.Menu;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Permission;
public interface IMenuPermissionService
{
    public Task<Result<MessageResponseModel>> AssignRolePermissionAsync(PermissionManagementViewModel permissionManagementModel, CancellationToken cancellationToken = default);
    Result<RolePermissionViewModel> GetAllMenuByRoleId(string roleId);
    Result<MenuModel> GetMenu();
}
