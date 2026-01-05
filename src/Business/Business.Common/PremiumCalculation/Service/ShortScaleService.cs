using SharedKernel.Helper;

namespace Business.Common.PremiumCalculation.Service;
public class ShortScaleService : IShortScaleService
{
    private decimal? _shortScaleRate;

    public void Initialize(decimal shortScaleRate)
    {
        _shortScaleRate = shortScaleRate;
    }

    public decimal CalculateShortScaleAmount(decimal amount)
    {
        if (_shortScaleRate == null)
        {
            throw new ArgumentNullException("ShortScaleRate", "Short-scale rate is not set");
        }

        return (amount * _shortScaleRate.Value).RoundToFourPrecisions();
    }
}
