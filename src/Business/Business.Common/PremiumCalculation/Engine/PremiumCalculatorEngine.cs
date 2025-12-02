using Data.Entities.Tenant;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public class PremiumCalculatorEngine : IPremiumCalculatorEngine
{
    private readonly IFormulaEvaluationEngine _formulaEngine;
    private readonly IRuleExecutionEngine _ruleEngine;
    private readonly Service.IRateTableService _rateTableService;

    public PremiumCalculatorEngine(
        IFormulaEvaluationEngine formulaEngine,
        IRuleExecutionEngine ruleEngine,
        Service.IRateTableService rateTableService)
    {
        _formulaEngine = formulaEngine;
        _ruleEngine = ruleEngine;
        _rateTableService = rateTableService;
    }

    public async Task<Result<PremiumCalculationResultModel>> CalculatePremiumAsync(
        CreatePolicyViewModel model, 
        PremiumCalculationConfiguration config)
    {
        var result = new PremiumCalculationResultModel
        {
            SumInsuredAmount = model.FullSumInsured,
            Days = model.PolicyPeriodInDays ?? 365
        };

        // Load parameters into a dictionary for easy access
        var parameters = config.Parameters
            .Where(p => !p.IsDeleted)
            .ToDictionary(p => p.ParameterKey, p => DeserializeParameterValue(p));

        // Build context for formula evaluation
        var context = BuildContext(result, model, parameters);

        // Execute rules in priority order
        var rules = config.Rules
            .Where(r => r.IsActive && !r.IsDeleted)
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.RuleName)
            .ToList();

        var ruleResult = await _ruleEngine.ExecuteRulesInOrderAsync(rules, result, model);
        if (!ruleResult.IsSuccess)
        {
            return Result<PremiumCalculationResultModel>.Failed(ruleResult.Error);
        }

        result = ruleResult.Data;

        // Apply any final calculations based on parameters
        await ApplyParameterBasedCalculations(result, parameters, context);

        return Result<PremiumCalculationResultModel>.Success(result);
    }

    public async Task<Result<PremiumCalculationResultModel>> ApplyRulesAsync(
        PremiumCalculationResultModel result, 
        List<PremiumCalculationRule> rules, 
        CreatePolicyViewModel model)
    {
        return await _ruleEngine.ExecuteRulesInOrderAsync(rules, result, model);
    }

    public async Task<Result<decimal>> EvaluateFormulaAsync(
        string expression, 
        Dictionary<string, object> variables)
    {
        return await _formulaEngine.EvaluateAsync(expression, variables);
    }

    public async Task<Result<decimal?>> LookupRateTableAsync(
        string tableName, 
        string lookupKey, 
        object lookupValue, 
        string configurationId)
    {
        var rateResult = await _rateTableService.GetRateAsync(configurationId, tableName, lookupKey, lookupValue);
        if (!rateResult.IsSuccess)
        {
            return Result<decimal?>.Success(null);
        }
        return Result<decimal?>.Success(rateResult.Data);
    }

    private Dictionary<string, object> BuildContext(
        PremiumCalculationResultModel result, 
        CreatePolicyViewModel model, 
        Dictionary<string, object> parameters)
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

        // Add parameters to context
        foreach (var param in parameters)
        {
            context[param.Key] = param.Value;
        }

        return context;
    }

    private async Task ApplyParameterBasedCalculations(
        PremiumCalculationResultModel result, 
        Dictionary<string, object> parameters, 
        Dictionary<string, object> context)
    {
        // Apply basic premium calculation if parameter exists
        if (parameters.TryGetValue("BasicPremiumRate", out var basicRateObj))
        {
            var basicRate = Convert.ToDecimal(basicRateObj);
            if (result.BasicPremium == 0)
            {
                result.BasicPremium = result.SumInsuredAmount * basicRate / 100;
            }
        }

        // Apply short scale if parameter exists
        if (parameters.TryGetValue("ShortScaleRate", out var shortScaleObj))
        {
            var shortScaleRate = Convert.ToDecimal(shortScaleObj);
            if (result.Days < 365)
            {
                result.BasicPremium = result.BasicPremium * shortScaleRate / 100;
            }
        }

        // Calculate gross premium if not already set
        if (result.GrossPremiumAmount == 0)
        {
            result.GrossPremiumAmount = result.BasicPremium;
        }

        await Task.CompletedTask;
    }

    private object DeserializeParameterValue(Data.Entities.Tenant.PremiumCalculationParameter parameter)
    {
        return parameter.DataType.ToLower() switch
        {
            "decimal" => decimal.Parse(parameter.Value),
            "int" => int.Parse(parameter.Value),
            "bool" => bool.Parse(parameter.Value),
            "string" => parameter.Value,
            _ => parameter.Value
        };
    }
}

