using Microsoft.AspNetCore.Http;
using Models.Common;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Auth;

public interface IAdminAuthService
{
    Task<Result<LoginCustomerResponseModel>> LoginAsync(AdminLoginRequestModel requestModel);
    Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaAdminRequestModel requestModel);
    Task<Result<MessageResponseModel>> RefreshTokenAsync();
    Task<Result<MessageResponseModel>> LogoutAsync(HttpResponse response);
}

