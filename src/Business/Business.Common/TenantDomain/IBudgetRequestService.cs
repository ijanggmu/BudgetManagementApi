using Models.BeemaEdgeApi.BudgetRequest;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetRequestService
{
    Task<Result<List<BudgetRequestResponseDto>>> GetAllAsync(BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<BudgetRequestResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<BudgetRequestResponseDto>> CreateAsync(CreateBudgetRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<BudgetRequestResponseDto>> ApproveAsync(string id, ApproveBudgetRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<BudgetRequestResponseDto>> RejectAsync(string id, RejectBudgetRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportAsync(BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default);
}
