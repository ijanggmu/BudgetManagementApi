using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Audit.UserActivites;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using SharedKernel.Operation;

namespace AdminPortalApi.Controllers.V1.SystemLog;
public class AccessLogResponseModel
{
    public string UserName { get; set; }
    public string IpAddress { get; set; }
    public string At { get; set; }
    public string EndAt { get; set; }

    public string RequestHost { get; set; }
    public string UserAgent { get; set; }

    public string RequestPath { get; set; }
    public string RequestPathAlias { get; set; }
    public string RequestMethod { get; set; }
    public string RequestQueryString { get; set; }
    public string RequestBody { get; set; }

    public int ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public string CorrelationId { get; set; }
    public double ResponseTimeInMS { get; set; }
    public string Module { get; set; }
    public string RequestHeader { get; set; }
}
public class SystemLogService : ISystemLogService
{
    private readonly ISieveExtension _sieveExtension;

    private readonly AuditDataContext _auditContext;
    public SystemLogService(ISieveExtension sieveExtension,
        AuditDataContext auditContext)
    {
        _sieveExtension = sieveExtension;
        _auditContext = auditContext;
    }
    public async Task<Result<List<AccessLogResponseModel>>> GetAllSystemAccessLogAsync(CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
    {
        var query = _auditContext.UserActivities;

        var defaultSort = $"-{nameof(UserActivity.At)}";

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, searchModel, null, defaultSort);

        var response = await result.Select(x => new AccessLogResponseModel
        {
            UserName = x.UserName,
            IpAddress = x.IpAddress,
            At = x.At.ToString(),
            Module = x.Module,
            RequestBody = x.RequestBody,
            RequestHost = x.RequestHost,
            RequestMethod = x.RequestMethod,
            RequestPathAlias = x.RequestPathAlias,
            RequestQueryString = x.RequestQueryString,
            ResponseBody = x.ResponseBody,
            ResponseStatusCode = x.ResponseStatusCode,
            UserAgent = x.UserAgent,
            RequestPath = x.RequestPath,
            CorrelationId = x.CorrelationId,
            ResponseTimeInMS = x.ResponseTime,
            EndAt = x.EndAt.ToString(),
            RequestHeader = x.RequestHeader,

        }).ToListAsync(cancellationToken);

        return Result<List<AccessLogResponseModel>>.Success(response, new Pagination
        {
            TotalItems = totalCount,
            CurrentPage = searchModel.PageNumber,
            PageSize = searchModel.PageSize,
            TotalPages = totalPage,
        });

    }
}
