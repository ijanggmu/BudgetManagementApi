using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Identity;
using Models.Common;
using Models.Common.Policy.Manufacturer;
using Models.Common.Policy.Policy;
using Models.Common.Policy.ThirdPartyApi;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Policy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Policy;
public class PolicyService : IPolicyService
{
    private readonly ApplicationDataContext _context;
    private readonly ICoreApiService _coreApiService;
    private readonly IUserProfileService _profileService;
    private readonly UserManager<ApplicationUser> _userManager;


    public PolicyService(ApplicationDataContext context, ICoreApiService coreApiService, IUserProfileService profileService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _coreApiService = coreApiService;
        _profileService = profileService;
        _userManager = userManager;
    }

    public async Task<Result<PolicyResponseModel>> BuyPolicyAsync(BuyPolicyRequestModel requestModel)
    {
        var result = await _coreApiService.BuyPolicyAsync(requestModel);
        return Result<PolicyResponseModel>.Success(result.Data);

    }

    public async Task<Result<PolicyResponseModel>> RenewPolicyAsync(BuyPolicyRequestModel requestModel)
    {
        var result = await _coreApiService.RenewPolicyAsync(requestModel);
        return Result<PolicyResponseModel>.Success(result.Data);

    }

    public async Task<Result<ThirdPartyPolicyResponseModel>> GetPolicyDetailsAsync(string draftNo)
    {
        var result = await _coreApiService.GetPolicyDetailsAsync(draftNo);
        if (result == null)
            Result<ThirdPartyPolicyResponseModel>.Failed("Policy details not found.");
        return Result<ThirdPartyPolicyResponseModel>.Success(result);
    }

    public async Task<string> PrintPolicyAsync(PrintPolicyCoreRequestModel requestModel)
    {
        var result = await _coreApiService.PrintPolicyAsync(requestModel);
        return result;
    }
    public async Task<Result<IEnumerable<PolicyResponseModel>>> GetMyPoliciesAsync(CommonPaginationRequestModel requestModel)
    {
        var result = await _coreApiService.GetPolicyAsync();
        return Result<IEnumerable<PolicyResponseModel>>.Success(result.Data);

    }

    public async Task<Result<List<PolicyIssuanceViewModel>>> GetPolicyList(string partyCode)
    {
        var result = await _coreApiService.GetPolicyList(partyCode);
        return Result<List<PolicyIssuanceViewModel>>.Success(result);
    }
    public async Task<Result<List<ManufacturerImportExcelData>>> GetAllManufacture(ManufactureByClassNameRequestModel requestModel)
    {
        var result = await _coreApiService.GetAllManufacture(requestModel.ClassName);
        return Result<List<ManufacturerImportExcelData>>.Success(result);
    }

    public async Task<Result<List<PolicyDetailResponse>>> PolicyDetails(PolicyDetailRequest model)
    {
        var result = await _coreApiService.PolicyDetails(model);
        return Result<List<PolicyDetailResponse>>.Success(result);
    }


}

