using System.Threading;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationRuleService
{
    Task<Result<List<PremiumCalculationRuleDto>>> GetRulesByConfigurationIdAsync(string configurationId, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRuleDto>> GetRuleByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRuleDto>> CreateRuleAsync(string configurationId, CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRuleDto>> UpdateRuleAsync(string id, CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteRuleAsync(string id, CancellationToken cancellationToken = default);
}

