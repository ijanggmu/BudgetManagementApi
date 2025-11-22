using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IFodoRegistrationService
{
    Task<Result<MessageResponseModel>> RegisterAsync(RegisterFodoRequestModel requestModel);
    Task<Result<MessageResponseModel>> VerifyFodoOtpAsync(VerifyFodoOtpRequestModel requestModel);
}

