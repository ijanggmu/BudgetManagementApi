using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationRuleService
{
    Task<Result<List<PremiumCalculationRuleDto>>> GetRulesByConfigurationIdAsync(string configurationId);
    Task<Result<PremiumCalculationRuleDto>> GetRuleByIdAsync(string id);
    Task<Result<PremiumCalculationRuleDto>> CreateRuleAsync(string configurationId, CreatePremiumCalculationRuleDto dto);
    Task<Result<PremiumCalculationRuleDto>> UpdateRuleAsync(string id, CreatePremiumCalculationRuleDto dto);
    Task<Result<bool>> DeleteRuleAsync(string id);
}

