using System.Threading;
using Data.Context;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerClaim;
using Models.WebApi.Claim;
using SharedKernel.Operation;
using static Infrastructure.CoreApi.CoreApi.ICoreApiService;

namespace Business.BeemaEdgeApi.Claim;
public class CustomerClaimService(ICoreApiService coreApiService, ApplicationDataContext _context,
    IUserProfileService _ipersonAccessor) : ICustomerClaimService
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

        var policyDraft = await _context.PolicyDrafts
        .Include(x => x.Customer)
        .ThenInclude(c => c.User)
        .Include(x => x.Customer)
        .ThenInclude(c => c.Addresses)
        .Where(x => x.PolicyNumber == model.PolicyNumber && x.Id == model.Id && x.Customer.UserId == userId)
        .Select(x => new CoreClaimIntimationRequestModel()
        {

            InsuredName = x.Customer.FullName,
            InsuredPhoneNumber = x.Customer.User.PhoneNumber,
            ClassId = x.PortfolioAlias
        }).FirstOrDefaultAsync();

        if (policyDraft == null)
        {
            return Result<CustomerClaimResponseModel>.Failed("Policy draft not found or you don't have permission to claim.");
        }
        var coreClaimModel = new CoreClaimIntimationRequestModel
        {
            Id = model.Id,
            PolicyNumber = model.PolicyNumber,
            DocumentNumber = model.DocumentNumber,
            ClaimAmount = model.ClaimAmount,
            Remarks = model.Remarks,
            CancelledRemarks = model.CancelledRemarks,
            OccuranceDateTime = model.OccuranceDateTime,
            OccuranceDateTimeNepali = model.OccuranceDateTimeNepali,
            InsuredName = policyDraft.InsuredName,
            ClassId = model.Class,
            CreatedAt = DateTime.UtcNow,

            ClaimIntimationE2ETravelModel = new ClaimIntimationE2ETravelModel
            {
                ClaimAmountUSD = model.ClaimIntimationE2ETravelModel.ClaimAmountUSD,
                OccuranceCountry = model.ClaimIntimationE2ETravelModel.OccuranceCountry,
                ClaimFor = "Self",
                ClaimantName = policyDraft.InsuredName
            },


        };

        policyDraft.EffectiveDate = DateTime.UtcNow;
        var result = await coreApiService.ApplyClaimAsync(coreClaimModel);

        if (result == null || result.Data == null)
        {
            return Result<CustomerClaimResponseModel>.Failed("Failed to apply claim, please try again later.");
        }

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
