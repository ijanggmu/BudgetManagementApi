using Data.Entities.Tenant;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public interface IRuleExecutionEngine
{
    Task<Result<PremiumCalculationResultModel>> ExecuteRuleAsync(
        PremiumCalculationRule rule,
        PremiumCalculationResultModel result,
        CreatePolicyViewModel model);

    Task<Result<bool>> EvaluateConditionAsync(string conditionJson, Dictionary<string, object> context);

    Task<Result<PremiumCalculationResultModel>> ExecuteRulesInOrderAsync(
        List<PremiumCalculationRule> rules,
        PremiumCalculationResultModel result,
        CreatePolicyViewModel model);
}

