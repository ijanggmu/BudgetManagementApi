using Models.BeemaEdgeApi.Budget;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface INepaliFiscalYearService
{
    Task<Result<List<NepaliFiscalYearResponseDto>>> GetAllAsync(NepaliFiscalYearListRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<NepaliFiscalYearResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<NepaliFiscalYearResponseDto>> CreateAsync(CreateNepaliFiscalYearDto dto, CancellationToken cancellationToken = default);
    Task<Result<NepaliFiscalYearResponseDto>> UpdateAsync(string id, UpdateNepaliFiscalYearDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
