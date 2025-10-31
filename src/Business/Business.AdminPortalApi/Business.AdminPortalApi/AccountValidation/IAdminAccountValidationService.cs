using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.AccountValidation;

public interface IAdminAccountValidationService
{
    Task<Result<AvailabilityResponseModel>> IsUsernameTakenAsync(string username);
    Task<Result<AvailabilityResponseModel>> IsEmailTakenAsync(string email);
    Task<Result<AvailabilityResponseModel>> IsPhoneNumberTakenAsync(string phoneNumber);
}
