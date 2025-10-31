using Data.Context;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.AccountValidation;

public class AdminAccountValidationService(ApplicationDataContext dbContext) : IAdminAccountValidationService
{
    public async Task<Result<AvailabilityResponseModel>> IsUsernameTakenAsync(string username)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.UserName == username);

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }

    public async Task<Result<AvailabilityResponseModel>> IsEmailTakenAsync(string email)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.NormalizedEmail == email.ToUpper());

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }

    public async Task<Result<AvailabilityResponseModel>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber);

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }
}
