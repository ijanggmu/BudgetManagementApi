using Microsoft.AspNetCore.Http;
using Models.Common;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Auth;

public interface ICustomerAuthService
{
    Task<Result<LoginCustomerResponseModel>> LoginAsync(IndividualLoginRequestModel requestModel);
    Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaCustomerRequestModel requestModel);
    Task<Result<MessageResponseModel>> RefreshTokenAsync();
    Task<Result<MessageResponseModel>> LogoutAsync(HttpResponse response);
}

