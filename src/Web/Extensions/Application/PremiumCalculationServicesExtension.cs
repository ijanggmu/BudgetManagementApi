using Business.Common.PremiumCalculation.Abstract;
using Business.Common.PremiumCalculation.Calculator;
using Business.Common.PremiumCalculation.Calculator.Motor;
using Business.Common.PremiumCalculation.Configuration;
using Business.Common.PremiumCalculation.Engine;
using Business.Common.PremiumCalculation.Factory;
using Business.Common.PremiumCalculation.Service;
using Microsoft.Extensions.DependencyInjection;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;
using UnderwritingService.Calculation.PremiumCalculation.Factory;

namespace BeemaEdgeApi.Extensions.Application;

public static class PremiumCalculationServicesExtension
{
    public static IServiceCollection AddPremiumCalculationServices(this IServiceCollection services)
    {
        // Configuration Service
        services.AddScoped<IPremiumCalculationConfigurationService, PremiumCalculationConfigurationService>();

        // Parameter Service
        services.AddScoped<IPremiumCalculationParameterService, PremiumCalculationParameterService>();

        // Rule Service
        services.AddScoped<IPremiumCalculationRuleService, PremiumCalculationRuleService>();

        // Rate Table Service (Configuration)
        services.AddScoped<IPremiumCalculationRateTableService, PremiumCalculationRateTableService>();

        // Formula Evaluation Engine
        services.AddScoped<IFormulaEvaluationEngine, FormulaEvaluationEngine>();

        // Rule Execution Engine
        services.AddScoped<IRuleExecutionEngine, RuleExecutionEngine>();

        // Rate Table Service (for lookups)
        services.AddScoped<Business.Common.PremiumCalculation.Service.IRateTableService, Business.Common.PremiumCalculation.Service.RateTableService>();

        // Premium Calculator Engine
        services.AddScoped<IPremiumCalculatorEngine, PremiumCalculatorEngine>();

        // Configurable Premium Calculator
        services.AddScoped<ConfigurablePremiumCalculator>();

        // Factory (updated to use new calculator)
        services.AddScoped<IPremiumCalculatorFactory, UnderwritingService.Calculation.PremiumCalculation.Factory.PremiumCalculatorFactory>();

        services.AddScoped<IPolicyPremiumCalculator, MotorcyclePremiumCalculator>();
        services.AddScoped<IPolicyPremiumCalculatorFactory, PolicyPremiumCalculatorFactory>();


        return services;
    }
}

