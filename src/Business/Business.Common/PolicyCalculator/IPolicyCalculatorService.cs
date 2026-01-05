using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.ThirdPartyApi.e2e;
using SharedKernel.Operation;

namespace Business.Common.PolicyCalculator;

public interface IPolicyCalculatorService
{
    Task<Result<PremiumCalculationResponseModel>> CalculatePremiumAsync(PremiumCalculateRequestModel requestModel);
    Task<Decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel);
}
public interface IPolicyPremiumCalculatorService
{
    Task<Result<ICalculationPremiumJson>> CalculatePolicyPremiumAsync(PremiumCalculateRequestModel requestModel);
    Task<Decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel);
}

