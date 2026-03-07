using Models.BeemaEdgeApi.Memo;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IMemoService
{
    Task<Result<List<MemoResponseDto>>> GetAllAsync(MemoListRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MemoResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<MemoResponseDto>> GetByBudgetRequestIdAsync(string budgetRequestId, CancellationToken cancellationToken = default);
    Task<Result<MemoResponseDto>> CreateAsync(CreateMemoDto dto, CancellationToken cancellationToken = default);
    /// <summary>Creates a memo for an existing budget request (any status). Used when request and memo are created together.</summary>
    Task<Result<MemoResponseDto>> CreateForRequestAsync(string budgetRequestId, CreateMemoDto dto, CancellationToken cancellationToken = default);
    Task<Result<MemoResponseDto>> UpdateAsync(string id, UpdateMemoDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> GeneratePdfAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> GenerateDocxAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportToExcelAsync(MemoListRequestModel requestModel, CancellationToken cancellationToken = default);
}
