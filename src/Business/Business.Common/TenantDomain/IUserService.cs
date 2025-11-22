using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Identity;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IUserService
{
    Task<Result<ApplicationUser>> CreateAsync(CreateUserDto dto);
    Task<Result<List<ApplicationUser>>> ListAsync(CommonPaginationRequestModel requestModel);
    Task<Result<ApplicationUser>> GetByIdAsync(string id);
    Task<Result<ApplicationUser>> UpdateAsync(string id, UpdateUserDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}

public record CreateUserDto(string UserName, string Email, string? PhoneNumber, string Password, string? Role);
public record UpdateUserDto(string? Email, string? PhoneNumber, bool? IsDisabled);

