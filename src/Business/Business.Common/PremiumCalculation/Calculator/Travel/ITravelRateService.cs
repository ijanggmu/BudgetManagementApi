using Models.Common.Policy.Policy.Miscellaneous;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public interface ITravelRateService
{
    Task<bool> GenerateTravelUSDRateAsync(TravelRateRootobject request);
    Task<decimal> GetTravelHEOMIRateAsync(HEOMIRateRequestModel request);
    Task<decimal> GetTravelUSDRateAsync(TravelUSDRateRequestModel request);
}