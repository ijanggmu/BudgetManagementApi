using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.AccountValidation;

public interface ICustomerAccountValidationService
{
    Task<Result<AvailabilityResponseModel>> IsUsernameTakenAsync(string username);
    Task<Result<AvailabilityResponseModel>> IsEmailTakenAsync(string email);
    Task<Result<AvailabilityResponseModel>> IsPhoneNumberTakenAsync(string phoneNumber);
}
