namespace Business.Common.PremiumCalculation.Service;

public interface IShortScaleService
{
    decimal CalculateShortScaleAmount(decimal amount);
    void Initialize(decimal shortScaleRate);
}