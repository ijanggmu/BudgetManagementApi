using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.WebApi.Common.Premium;
using SharedKernel.Constant.Permission;
using SharedKernel.Operation;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

/// <summary>
/// Controller for calculating premium using the new configurable premium calculation engine
/// </summary>
[Route("api/v1/premiums")]
public class ConfigurablePremiumCalculatorController(
    IPremiumCalculatorFactory calculatorFactory) : BaseCommonApiController
{
    /// <summary>
    /// Calculate premium using configurable calculation engine
    /// </summary>
    /// <param name="model">Policy creation model with PortfolioAlias, FiscalYear, and other policy details</param>
    /// <returns>Premium calculation result</returns>
    [HttpPost("calculate")]
    [Permission(MenuPermissionConstant.PremiumOverviewView)]
    public async Task<IActionResult> CalculatePremium([FromBody] CreatePolicyViewModel model)
    {
        try
        {
            // Get calculator from factory (returns ConfigurablePremiumCalculator)
            var calculator = calculatorFactory.GetCalculator(model.PortfolioAlias ?? "");

            // Calculate premium using the new configurable calculator
            var result = await calculator.CalculatePremium(model);

            return HandleResult(Result<PremiumCalculationResultModel>.Success(result));
        }
        catch (ArgumentException ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                ex.Message, 
                400, 
                null, 
                System.Net.HttpStatusCode.BadRequest));
        }
        catch (InvalidOperationException ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                ex.Message, 
                404, 
                null, 
                System.Net.HttpStatusCode.NotFound));
        }
        catch (Exception ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                $"An error occurred while calculating premium: {ex.Message}", 
                500, 
                null, 
                System.Net.HttpStatusCode.InternalServerError));
        }
    }

    /// <summary>
    /// Calculate endorsement premium adjustment
    /// </summary>
    /// <param name="request">Request containing endorsement model and original premium calculation</param>
    /// <returns>Adjusted premium calculation result</returns>
    [HttpPost("calculate-endorsement")]
    [Permission(MenuPermissionConstant.PremiumOverviewView)]
    public async Task<IActionResult> CalculateEndorsementPremium(
        [FromBody] CalculateEndorsementPremiumRequestDto request)
    {
        try
        {
            // Get calculator from factory
            var calculator = calculatorFactory.GetCalculator(request.EndorsementModel.PortfolioAlias ?? "");

            // Calculate endorsement premium adjustment
            var result = await calculator.CalculateEndorsementPremium(
                request.EndorsementModel, 
                request.PremiumCalculation);

            return HandleResult(Result<PremiumCalculationResultModel>.Success(result));
        }
        catch (ArgumentException ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                ex.Message, 
                400, 
                null, 
                System.Net.HttpStatusCode.BadRequest));
        }
        catch (InvalidOperationException ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                ex.Message, 
                404, 
                null, 
                System.Net.HttpStatusCode.NotFound));
        }
        catch (Exception ex)
        {
            return HandleResult(Result<PremiumCalculationResultModel>.Failed(
                $"An error occurred while calculating endorsement premium: {ex.Message}", 
                500, 
                null, 
                System.Net.HttpStatusCode.InternalServerError));
        }
    }
}

