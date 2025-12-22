using System.Threading;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.AccountValidation;

public class CustomerAccountValidationService(ApplicationDataContext dbContext) : ICustomerAccountValidationService
{
    public async Task<Result<AvailabilityResponseModel>> IsUsernameTakenAsync(string username, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.UserName == username, cancellationToken);

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }

    public async Task<Result<AvailabilityResponseModel>> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.NormalizedEmail == email.ToUpper(), cancellationToken);

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }

    public async Task<Result<AvailabilityResponseModel>> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        return Result<AvailabilityResponseModel>.Success(new AvailabilityResponseModel
        {
            IsTaken = exists,
        });
    }
}
