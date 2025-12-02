using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Engine;

public class FormulaEvaluationEngine : IFormulaEvaluationEngine
{
    public async Task<Result<decimal>> EvaluateAsync(string expression, Dictionary<string, object> variables)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return Result<decimal>.Failed("Expression cannot be empty");
        }

        try
        {
            // Replace variable names with their values
            var processedExpression = expression;
            foreach (var variable in variables)
            {
                var pattern = $@"\b{Regex.Escape(variable.Key)}\b";
                var value = Convert.ToDecimal(variable.Value);
                processedExpression = Regex.Replace(processedExpression, pattern, value.ToString("F4"), RegexOptions.IgnoreCase);
            }

            // Use DataTable.Compute for safe evaluation
            var dataTable = new System.Data.DataTable();
            var result = dataTable.Compute(processedExpression, null);

            if (result == null || result == DBNull.Value)
            {
                return Result<decimal>.Failed("Expression evaluation returned null");
            }

            var decimalResult = Convert.ToDecimal(result);
            return await Task.FromResult(Result<decimal>.Success(decimalResult));
        }
        catch (Exception ex)
        {
            return Result<decimal>.Failed($"Error evaluating expression: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ValidateExpressionAsync(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return Result<bool>.Failed("Expression cannot be empty");
        }

        try
        {
            // Basic validation: check for balanced parentheses and valid operators
            var openParens = expression.Count(c => c == '(');
            var closeParens = expression.Count(c => c == ')');
            
            if (openParens != closeParens)
            {
                return Result<bool>.Failed("Unbalanced parentheses");
            }

            // Check for valid operators and structure
            var validPattern = @"^[0-9+\-*/().\s\w]+$";
            if (!Regex.IsMatch(expression, validPattern))
            {
                return Result<bool>.Failed("Expression contains invalid characters");
            }

            // Try to compile a test expression
            var testExpression = expression.Replace(" ", "");
            var dataTable = new System.Data.DataTable();
            
            // Replace variable names with test values
            var testVars = new Dictionary<string, object> { { "TestVar", 1.0m } };
            foreach (var variable in testVars)
            {
                var pattern = $@"\b{Regex.Escape(variable.Key)}\b";
                testExpression = Regex.Replace(testExpression, pattern, variable.Value.ToString()!, RegexOptions.IgnoreCase);
            }

            // Try to compute with test values
            try
            {
                dataTable.Compute(testExpression, null);
            }
            catch
            {
                return Result<bool>.Failed("Expression syntax is invalid");
            }

            return await Task.FromResult(Result<bool>.Success(true));
        }
        catch (Exception ex)
        {
            return Result<bool>.Failed($"Error validating expression: {ex.Message}");
        }
    }
}

