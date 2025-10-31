using Data.Context;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerClaim;
using Models.WebApi.Claim;
using SharedKernel.Operation;
using static Infrastructure.CoreApi.CoreApi.ICoreApiService;

namespace Business.AdminPortalApi.Claim;
public class AdminClaimService(ICoreApiService coreApiService, ApplicationDataContext _context,
    IUserProfileService _ipersonAccessor) : IAdminClaimService
{

    public async Task<Result<List<CustomerClaimResponseModel>>> GetCustomerClaimAsync(CommonPaginationRequestModel requestModel)
    {
        var result = await coreApiService.GetCustomerClaimsAsync();
        return Result<List<CustomerClaimResponseModel>>.Success(result.Data);
    }

    public async Task<Result<CustomerClaimResponseModel>> GetCustomerClaimByIdAsync(string id)
    {
        var result = await coreApiService.GetCustomerClaimDetailsAsync(id);
        return Result<CustomerClaimResponseModel>.Success(result.Data);
    }
    public async Task<Result<CustomerClaimResponseModel>> ApplyClaimAsync(ClaimIntimationRequestModel model)
    {
        var userId = _ipersonAccessor.GetUserId();

        var customer = await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        var claimIntimation = new CoreClaimIntimationRequestModel
        {
                Id = Guid.NewGuid().ToString(),
                PolicyNumber = model.PolicyNumber,
                DocumentNumber = model.DocumentNumber,
                ClaimAmount = model.ClaimAmount,
                Remarks = model.Remarks,
                OccuranceDateTime = model.OccuranceDateTime,
                OccuranceDateTimeNepali = model.OccuranceDateTimeNepali,

            InsuredName = customer.FullName,
            EffectiveDate = DateTime.Now,
            InsuredPhoneNumber = customer.User.PhoneNumber

        };
        var result = await coreApiService.ApplyClaimAsync(claimIntimation);
        return Result<CustomerClaimResponseModel>.Success(result.Data);
    }
    public async Task<Result<CustomerClaimResponseModel>> UpdateClaimAsync(string id)
    {
        var result = await coreApiService.UpdateClaimAsync(id);
        return Result<CustomerClaimResponseModel>.Success(result.Data);
    }

    public async Task<Result<CustomerClaimResponseModel>> GetHospitalNamesAsync(CommonPaginationRequestModel requestModel)
    {
        var payload = new CoreClaimPaginationModel()
        {
            Page = requestModel.PageNumber,
            PerPage = requestModel.PageSize
        };
        var result = await coreApiService.GetHospitalNamesAsync(payload);
        return Result<CustomerClaimResponseModel>.Success(result.Data);
    }
}
