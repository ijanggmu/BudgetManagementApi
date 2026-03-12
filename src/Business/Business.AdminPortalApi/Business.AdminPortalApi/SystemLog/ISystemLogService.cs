using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using SharedKernel.Operation;

namespace AdminPortalApi.Controllers.V1.SystemLog;

public interface ISystemLogService
{
    Task<Result<List<AccessLogResponseModel>>> GetAllSystemAccessLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default);
    Task<Result<List<AccessLogResponseModel>>> GetRecentActivityAsync(int limit, CancellationToken cancellationToken = default);
    Task<Result<List<AccessLogResponseModel>>> GetActivityLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default);
}
