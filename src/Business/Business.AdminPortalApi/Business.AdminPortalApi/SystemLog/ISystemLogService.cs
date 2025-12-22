using System.Threading;
using Models.Common;
using SharedKernel.Operation;

namespace AdminPortalApi.Controllers.V1.SystemLog;

public interface ISystemLogService
{
    Task<Result<List<AccessLogResponseModel>>> GetAllSystemAccessLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default);
}
