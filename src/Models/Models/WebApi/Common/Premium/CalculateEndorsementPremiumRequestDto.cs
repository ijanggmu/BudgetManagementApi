using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;

namespace Models.WebApi.Common.Premium;

public class CalculateEndorsementPremiumRequestDto
{
    public EndorsementViewModel EndorsementModel { get; set; } = default!;
    public PremiumCalculationResultModel PremiumCalculation { get; set; } = default!;
}

