using Models.BeemaEdgeApi.Department;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IDepartmentService
{
    Task<Result<List<DepartmentResponseDto>>> GetAllAsync(DepartmentListRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponseDto>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponseDto>> UpdateAsync(string id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
