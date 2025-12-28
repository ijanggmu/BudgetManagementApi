using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using SharedKernel.Operation;

namespace Business.Common.PolicyCalculator;

public interface IPolicyCalculatorService
{
    Task<Result<PremiumCalculationResponseModel>> CalculatePremiumAsync(PremiumCalculateRequestModel requestModel);
    Task<Decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel);
}
public interface IPolicyPremiumCalculatorService
{
    Task<Result<PremiumCalculationResultModel>> CalculatePolicyPremiumAsync(PremiumCalculateRequestModel requestModel);
    Task<Decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel);
}

