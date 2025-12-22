using System.Threading;
using Models.Common;
using Models.BeemaEdgeApi.Roles;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Role;
public interface IRoleService
{
    Result<List<string>> GetAllSystemRoles();
    Task<Result<List<string>>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<RoleResponseModel>>> GetAllRolesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> CreateRoleAsync(CreateRoleRequestModel model, CancellationToken cancellationToken = default);
    Task<Result<RoleResponseModel>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> UpdateRoleAsync(UpdateRoleRequestModel roleModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);
}
