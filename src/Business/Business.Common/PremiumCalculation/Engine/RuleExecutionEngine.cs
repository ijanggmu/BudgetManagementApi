using System.Text.Json;
using Data.Entities.Tenant;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public class RuleExecutionEngine : IRuleExecutionEngine
{
    private readonly IFormulaEvaluationEngine _formulaEngine;

    public RuleExecutionEngine(IFormulaEvaluationEngine formulaEngine)
    {
        _formulaEngine = formulaEngine;
    }

    public async Task<Result<PremiumCalculationResultModel>> ExecuteRuleAsync(
        PremiumCalculationRule rule, 
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model)
    {
        if (!rule.IsActive)
        {
            return Result<PremiumCalculationResultModel>.Success(result);
        }

        // Check condition first
        if (!string.IsNullOrEmpty(rule.Condition))
        {
            var context = BuildContext(result, model);
            var conditionResult = await EvaluateConditionAsync(rule.Condition, context);
            
            if (!conditionResult.IsSuccess || !conditionResult.Data)
            {
                // Condition not met, skip rule
                return Result<PremiumCalculationResultModel>.Success(result);
            }
        }

        // Execute rule based on type
        switch (rule.RuleType.ToLower())
        {
            case "formula":
                return await ExecuteFormulaRuleAsync(rule, result, model);
            case "discount":
                return await ExecuteDiscountRuleAsync(rule, result, model);
            case "validation":
                return await ExecuteValidationRuleAsync(rule, result, model);
            default:
                return Result<PremiumCalculationResultModel>.Success(result);
        }
    }

    public async Task<Result<bool>> EvaluateConditionAsync(string conditionJson, Dictionary<string, object> context)
    {
        try
        {
            var condition = JsonSerializer.Deserialize<Dictionary<string, object>>(conditionJson);
            if (condition == null)
            {
                return Result<bool>.Failed("Invalid condition JSON");
            }

            foreach (var kvp in condition)
            {
                var fieldName = kvp.Key;
                var conditionValue = kvp.Value;

                if (!context.ContainsKey(fieldName))
                {
                    return Result<bool>.Failed($"Field '{fieldName}' not found in context");
                }

                var fieldValue = context[fieldName];
                
                // Simple equality check for now
                // Can be extended for range checks, etc.
                if (conditionValue is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        // Handle range conditions like {"$gte": 18, "$lte": 65}
                        var rangeConditions = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(element.GetRawText());
                        if (rangeConditions != null)
                        {
                            var fieldDecimal = Convert.ToDecimal(fieldValue);
                            
                            if (rangeConditions.ContainsKey("$gte"))
                            {
                                var min = rangeConditions["$gte"].GetDecimal();
                                if (fieldDecimal < min) return Result<bool>.Success(false);
                            }
                            
                            if (rangeConditions.ContainsKey("$lte"))
                            {
                                var max = rangeConditions["$lte"].GetDecimal();
                                if (fieldDecimal > max) return Result<bool>.Success(false);
                            }
                        }
                    }
                    else
                    {
                        var expectedValue = element.GetRawText().Trim('"');
                        if (fieldValue?.ToString() != expectedValue)
                        {
                            return Result<bool>.Success(false);
                        }
                    }
                }
                else
                {
                    if (fieldValue?.ToString() != conditionValue?.ToString())
                    {
                        return Result<bool>.Success(false);
                    }
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failed($"Error evaluating condition: {ex.Message}");
        }
    }

    public async Task<Result<PremiumCalculationResultModel>> ExecuteRulesInOrderAsync(
        List<PremiumCalculationRule> rules, 
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model)
    {
        var sortedRules = rules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.RuleName)
            .ToList();

        foreach (var rule in sortedRules)
        {
            var ruleResult = await ExecuteRuleAsync(rule, result, model);
            if (!ruleResult.IsSuccess)
            {
                return ruleResult;
            }
            result = ruleResult.Data;
        }

        return Result<PremiumCalculationResultModel>.Success(result);
    }

    private async Task<Result<PremiumCalculationResultModel>> ExecuteFormulaRuleAsync(
        PremiumCalculationRule rule, 
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model)
    {
        if (string.IsNullOrEmpty(rule.Expression))
        {
            return Result<PremiumCalculationResultModel>.Failed("Formula expression is empty");
        }

        var context = BuildContext(result, model);
        var formulaResult = await _formulaEngine.EvaluateAsync(rule.Expression, context);
        
        if (!formulaResult.IsSuccess)
        {
            return Result<PremiumCalculationResultModel>.Failed(formulaResult.Error);
        }

        // Apply formula result to appropriate field based on rule name
        // This is a simplified implementation - can be extended
        if (rule.RuleName.Contains("BasicPremium", StringComparison.OrdinalIgnoreCase))
        {
            result.BasicPremium = formulaResult.Data;
        }
        else if (rule.RuleName.Contains("GrossPremium", StringComparison.OrdinalIgnoreCase))
        {
            result.GrossPremiumAmount = formulaResult.Data;
        }
        // Add more mappings as needed

        return Result<PremiumCalculationResultModel>.Success(result);
    }

    private async Task<Result<PremiumCalculationResultModel>> ExecuteDiscountRuleAsync(
        PremiumCalculationRule rule, 
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model)
    {
        if (string.IsNullOrEmpty(rule.Expression))
        {
            return Result<PremiumCalculationResultModel>.Failed("Discount expression is empty");
        }

        var context = BuildContext(result, model);
        var discountResult = await _formulaEngine.EvaluateAsync(rule.Expression, context);
        
        if (!discountResult.IsSuccess)
        {
            return Result<PremiumCalculationResultModel>.Failed(discountResult.Error);
        }

        // Apply discount
        result.GrossPremiumAmount -= discountResult.Data;
        if (result.GrossPremiumAmount < 0)
        {
            result.GrossPremiumAmount = 0;
        }

        return Result<PremiumCalculationResultModel>.Success(result);
    }

    private async Task<Result<PremiumCalculationResultModel>> ExecuteValidationRuleAsync(
        PremiumCalculationRule rule, 
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model)
    {
        // Validation rules typically don't modify the result, just validate
        // Can throw or return error if validation fails
        return Result<PremiumCalculationResultModel>.Success(result);
    }

    private Dictionary<string, object> BuildContext(PremiumCalculationResultModel result, CreatePolicyViewModel model)
    {
        var context = new Dictionary<string, object>
        {
            { "SumInsured", result.SumInsuredAmount },
            { "BasicPremium", result.BasicPremium },
            { "GrossPremium", result.GrossPremiumAmount },
            { "Days", model.PolicyPeriodInDays ?? 365 },
            { "EffectiveDate", model.EffectiveDate },
            { "ExpiryDate", model.ExpiryDate }
        };

        // Add more context variables as needed based on model properties
        return context;
    }
}

