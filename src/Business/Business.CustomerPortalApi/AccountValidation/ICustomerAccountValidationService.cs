using System.Threading;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.AccountValidation;

public interface ICustomerAccountValidationService
{
    Task<Result<AvailabilityResponseModel>> IsUsernameTakenAsync(string username, CancellationToken cancellationToken = default);
    Task<Result<AvailabilityResponseModel>> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<AvailabilityResponseModel>> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
