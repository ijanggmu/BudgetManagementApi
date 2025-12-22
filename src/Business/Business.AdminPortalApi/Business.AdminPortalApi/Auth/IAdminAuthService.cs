using System.Threading;
using Microsoft.AspNetCore.Http;
using Models.Common;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Auth;

public interface IAdminAuthService
{
    Task<Result<LoginAdminResponseModel>> LoginAsync(AdminLoginRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaAdminRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> RefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> LogoutAsync(HttpResponse response, CancellationToken cancellationToken = default);
}

