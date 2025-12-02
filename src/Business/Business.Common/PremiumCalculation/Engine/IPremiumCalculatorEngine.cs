using Data.Entities.Tenant;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public interface IPremiumCalculatorEngine
{
    Task<Result<PremiumCalculationResultModel>> CalculatePremiumAsync(
        CreatePolicyViewModel model, 
        PremiumCalculationConfiguration config);
    
    Task<Result<PremiumCalculationResultModel>> ApplyRulesAsync(
        PremiumCalculationResultModel result, 
        List<PremiumCalculationRule> rules, 
        CreatePolicyViewModel model);
    
    Task<Result<decimal>> EvaluateFormulaAsync(
        string expression, 
        Dictionary<string, object> variables);
    
    Task<Result<decimal?>> LookupRateTableAsync(
        string tableName, 
        string lookupKey, 
        object lookupValue, 
        string configurationId);
}

