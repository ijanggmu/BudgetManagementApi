using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public interface IFormulaEvaluationEngine
{
    Task<Result<decimal>> EvaluateAsync(string expression, Dictionary<string, object> variables);
    Task<Result<bool>> ValidateExpressionAsync(string expression);
}

