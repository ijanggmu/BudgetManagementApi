using Models.BeemaEdgeApi.BudgetReport;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetReportService
{
    Task<Result<BudgetReportResponseDto>> GetReportAsync(BudgetReportRequestModel requestModel, CancellationToken cancellationToken = default);
}
