using Models.BeemaEdgeApi.Memo;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IMemoTemplateService
{
    Task<Result<List<MemoTemplateResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<MemoTemplateResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<MemoTemplateResponseDto>> CreateAsync(CreateMemoTemplateDto dto, CancellationToken cancellationToken = default);
    Task<Result<MemoTemplateResponseDto>> UpdateAsync(string id, UpdateMemoTemplateDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
