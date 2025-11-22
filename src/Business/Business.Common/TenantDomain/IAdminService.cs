using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IAdminService
{
    Task<Result<List<AdminResponseDto>>> GetAdminsForAdminAsync(string? tenantId = null);
    Task<Result<AdminResponseDto>> GetAdminByIdAsync(string id);
    Task<Result<AdminResponseDto>> CreateAsync(CreateAdminDto dto);
    Task<Result<AdminResponseDto>> UpdateAsync(string id, UpdateAdminDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}

