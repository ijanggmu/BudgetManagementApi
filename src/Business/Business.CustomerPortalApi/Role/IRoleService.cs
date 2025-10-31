using Models.Common;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Role;
public interface IRoleService
{
    Result<List<string>> GetAllSystemRoles();
    Task<Result<List<string>>> GetAllRoleNamesAsync();
    Task<Result<List<RoleResponseModel>>> GetAllRolesAsync(CommonPaginationRequestModel requestModel);
    Task<Result<MessageResponseModel>> CreateRoleAsync(CreateRoleRequestModel model);
    Task<Result<RoleResponseModel>> GetRoleByIdAsync(string roleId);
    Task<Result<MessageResponseModel>> UpdateRoleAsync(UpdateRoleRequestModel roleModel);
    Task<Result<MessageResponseModel>> DeleteRoleAsync(string roleId);
}
