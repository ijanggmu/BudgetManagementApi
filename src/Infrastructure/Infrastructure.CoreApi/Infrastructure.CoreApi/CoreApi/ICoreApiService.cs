using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Common;
using Models.Common.Location;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Manufacturer;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.ThirdPartyApi;
using Models.Common.Province;
using Models.CoreApi.Agent;
using Models.BeemaEdgeApi.Bank;
using Models.BeemaEdgeApi.Customer.CustomerClaim;
using Models.BeemaEdgeApi.Customer.Payment;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Claim;
using Models.WebApi.Customer.Policy;
using Models.WebApi.Individual;
using Models.WebApi.Policy;
using Refit;
using SharedKernel.Operation;

namespace Infrastructure.CoreApi.CoreApi;

public interface ICoreApiService
{

    [Post("/api/v1/Agent/SaveDraft")]
    Task<Result<PolicyResponseModel>> SaveDraftAsync(PolicyResponseModel policyDto);

    [Post("/api/v1/Agent/CheckAgent")]
    Task<Result<List<string>>> CheckAgentAsync([Body] CheckAgentRequestModel requestModel);

    [Get("/api/v1/Policy/GetPolicy")]
    Task<Result<List<PolicyResponseModel>>> GetPolicyAsync();

    [Get("/api/v1/Policy/GetPolicyList")]
    Task<List<PolicyIssuanceViewModel>> GetPolicyList([FromQuery] string partyCode);

    [Get("/api/v1/Policy/GetDraftPolicies")]
    Task<Result<List<PolicyResponseModel>>> GetDraftPoliciesAsync();

    [Get("/api/v1/Policy/GetAllManufactureByClass")]
    Task<List<ManufacturerImportExcelData>> GetAllManufacture([Query] string className);

    [Get("/api/v1/Policy/GetAllManufacture")]
    Task<List<ManufacturerViewModel>> GetAllManufacture();


    [Post("/api/v1/Policy/PolicyDetails")]
    Task<List<PolicyDetailResponse>> PolicyDetails(PolicyDetailRequest model);

    [Get("/api/v1/Policy/BuyPolicy")]
    Task<Result<PolicyResponseModel>> BuyPolicyAsync(BuyPolicyRequestModel requestModel);

    [Get("/api/v1/Policy/RenewPolicy")]
    Task<Result<PolicyResponseModel>> RenewPolicyAsync(BuyPolicyRequestModel requestModel);

    [Get("/api/v1/Policy/GetPolicyDetails")]
    Task<ThirdPartyPolicyResponseModel> GetPolicyDetailsAsync([FromQuery] string draftNo);

    [Get("/api/v1/Policy/PrintPolicy")]
    Task<string> PrintPolicyAsync([Query] PrintPolicyCoreRequestModel requestModel);

    [Post("/api/v3/Policy/CreatePolicy")]
    Task<ThirdPartyPolicyCreateResponse> CreatePolicyAsync(CorePolicyCreateViewModel requestModel);

    #region CustomerClaim

    [Get("/api/v1/Claim/CustomerClaims")]
    Task<Result<List<CustomerClaimResponseModel>>> GetCustomerClaimsAsync();

    [Get("/api/v1/Claim/CustomerClaim/{id}")]
    Task<Result<CustomerClaimResponseModel>> GetCustomerClaimDetailsAsync(string id);

    [Post("/api/v1/Claim/CreateIntimationNewPortfolio")]
    Task<CoreApiResponse<CustomerClaimResponseModel>> ApplyClaimAsync(CoreClaimIntimationRequestModel model);

    [Put("/api/v1/Claim/UpdateIntimation")]
    Task<CoreApiResponse<CustomerClaimResponseModel>> UpdateClaimAsync(string id);

    [Get("/api/v1/Claim/GetClaimHospitalNames")]
    Task<Result<CustomerClaimResponseModel>> GetHospitalNamesAsync(CoreClaimPaginationModel requestModel);
    public class CoreClaimPaginationModel
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
    }
    #endregion

    #region BusinessController

    [Get("/api/v1/Business/GetBusinessTypes")]
    Task<Result<List<string>>> GetBusinessTypesAsync();

    #endregion

    #region ClassController

    [Get("/api/v1/Class/List")]
    Task<Result<List<string>>> GetClassListAsync();

    [Get("/api/v1/Class/ClassesByPortfolio")]
    Task<Result<List<string>>> GetClassesByPortfolioAsync();

    #endregion

    #region Location

    [Get("/api/v1/Location/GetProvinces")]
    Task<List<ProvinceViewModel>> GetProvincesAsync();

    [Post("/api/v1/Location/GetDistricts")]
    Task<List<DistrictViewModel>> GetDistrictsAsync(string provinceName);

    [Post("/api/v1/Location/GetMunicipalities")]
    Task<List<MunicipalityViewModel>> GetMunicipalitiesAsync(string districtName);

    [Post("/api/v1/Location/GetWards")]
    Task<List<WardViewModel>> GetWardsAsync(string municipalityName);
    #endregion


    #region Payment

    [Get("/api/v1/Payment/GetPaymentHistory")]
    Task<Result<PaymentResponseModel>> GetPaymentHistoryAsync();

    [Get("/api/v1/Payment/MakePayment")]
    Task<Result<PaymentResponseModel>> MakePaymentAsync();

    #endregion

    #region #portfolio

    [Get("/api/v1/Portfolio/GetPortfolioList")]
    Task<List<string>> GetPortfolioListAsync();

    [Get("/api/v1/Portfolio/GetGroupedByPortfolio")]
    Task<Result<Dictionary<string, List<string>>>> GetGroupedByPortfolioAsync();


    #endregion

    [Post("/api/v1/Policy/GetCalculationPremiumDetails")]
    Task<PremiumCalculationResponseModel> CalculatePolicyPremiumAsync(CreatePolicyViewModel requestModel);

    [Post("/api/v1/TravelRate/CalculateUsdRate")]
    Task<Decimal> CalculateUsdRateAsync(TravelUSDRateRequestModel requestModel);

    [Post("/api/v1/Policy/IndividualCheck")]
    Task<IndividualCheckResponseModel> IndividualCustomerCheck(IndividualCustomerCheckRequestModel requestModel);

    [Post("/api/v1/Policy/CreateIndividual")]
    Task<HttpResponseMessage> CreateIndividual(CreateIndividualRequestModel requestModel);

    [Post("/api/v2/Policy/CreateIndividualV2")]
    Task<IndividualResponseModel> CreateIndividualV2(ContactViewModel requestModel);

    [Get("/api/v1/Policy/GetBankList")]
    Task<List<BankResponseViewModel>> GetBankList();

    [Get("/api/v1/Policy/GetBranchList")]
    Task<IEnumerable<BranchDetailViewModel>> GetBranchList();

    [Get("/api/v1/Policy/UpdateIndividual")]
    Task<IndividualResponseModel> UpdateIndividual(ContactViewModel requestModel);

    [Post("/api/v1/policy/CreatePolicy")]
    Task<ThirdPartyPolicyResponseModel> CreateMerchantPolicy(CorePolicyCreateViewModel requestModel);


    #region
    [Post("/api/v1/Agent/CheckAgent")]
    Task<AgentResponseModel> GetAgentByAgentCodeAsync(GetAgentByCodeRequestModel requestModel);
    #endregion
}
