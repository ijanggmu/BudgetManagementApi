using Models.BeemaEdgeApi.ApprovalConfig;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IApprovalConfigService
{
    Task<Result<List<ApprovalConfigResponseDto>>> GetAllAsync(ApprovalConfigListRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<ApprovalConfigResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<ApprovalConfigResponseDto>> GetByDepartmentIdAsync(string departmentId, CancellationToken cancellationToken = default);
    Task<Result<ApprovalConfigResponseDto>> CreateAsync(CreateApprovalConfigDto dto, CancellationToken cancellationToken = default);
    Task<Result<ApprovalConfigResponseDto>> UpdateAsync(string id, UpdateApprovalConfigDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<ApprovalConfigImportResultDto>> ImportFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default);
    Task<Result<List<ApproverRoleItemDto>>> GetApproverRolesAsync(CancellationToken cancellationToken = default);
}
