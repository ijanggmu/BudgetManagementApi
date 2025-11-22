using Microsoft.AspNetCore.Http;
using Models.BeemaEdgeApi.Fodo;
using Models.BeemaEdgeApi.Identity;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IFodoAuthService
{
    Task<Result<LoginCustomerResponseModel>> LoginAsync(AgentLoginRequestModel requestModel);
    Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaCustomerRequestModel requestModel);
    Task<Result<MessageResponseModel>> RefreshTokenAsync();
    Result<MessageResponseModel> Logout(HttpResponse response);
}

