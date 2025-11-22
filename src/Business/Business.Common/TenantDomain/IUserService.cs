using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IUserService
{
    Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto);
    Task<Result<List<UserResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel);
    Task<Result<UserResponseDto>> GetByIdAsync(string id);
    Task<Result<UserResponseDto>> UpdateAsync(string id, UpdateUserDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}

public record CreateUserDto(string UserName, string Email, string? PhoneNumber, string Password, string? Role);
public record UpdateUserDto(string? Email, string? PhoneNumber, bool? IsDisabled);

