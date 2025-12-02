using Business.Common.PremiumCalculation.Configuration;
using Business.Common.PremiumCalculation.Engine;
using Data.Context;
using Data.Entities.Tenant;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Policy;
using Microsoft.EntityFrameworkCore;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator;

public class ConfigurablePremiumCalculator : IPremiumCalculator
{
    private readonly IPremiumCalculationConfigurationService _configService;
    private readonly IPremiumCalculatorEngine _engine;
    private readonly ApplicationDataContext _db;

    public ConfigurablePremiumCalculator(
        IPremiumCalculationConfigurationService configService,
        IPremiumCalculatorEngine engine,
        ApplicationDataContext db)
    {
        _configService = configService;
        _engine = engine;
        _db = db;
    }

    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        if (string.IsNullOrEmpty(model.PortfolioAlias))
        {
            throw new ArgumentException("PortfolioAlias is required", nameof(model));
        }

        if (string.IsNullOrEmpty(model.FiscalYear))
        {
            throw new ArgumentException("FiscalYear is required", nameof(model));
        }

        // Get configuration for this portfolio and fiscal year
        var configResult = await _configService.GetConfigurationAsync(
            model.PortfolioAlias, 
            model.FiscalYear, 
            model.EffectiveDate);

        if (!configResult.IsSuccess || configResult.Data == null)
        {
            throw new InvalidOperationException(
                $"Configuration not found for Portfolio: {model.PortfolioAlias}, FiscalYear: {model.FiscalYear}. " +
                $"Error: {configResult.Error}");
        }

        // Convert DTO to entity (we need the entity for the engine)
        // For now, we'll need to reload it from DB or pass the DTO
        // Let's reload from DB to get the full entity with navigation properties
        var configEntityResult = await _configService.GetConfigurationByIdAsync(configResult.Data.Id);
        if (!configEntityResult.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to load configuration entity: {configEntityResult.Error}");
        }

        // We need to get the entity, but the service returns DTOs
        // Let's modify the approach - we'll need to load the entity directly in the engine
        // For now, let's create a helper method or modify the service to return entities
        // Actually, let's pass the configuration ID and let the engine load it
        
        // Since we can't easily convert DTO to entity, let's modify the engine to accept configuration ID
        // Or we can add a method to get the entity directly
        // For now, let's use a workaround - reload the entity in the engine
        
        // Actually, the best approach is to modify the engine to accept the DTO and work with it
        // But that requires changing the engine interface. Let's keep it simple for now
        // and reload the entity in a helper method
        
        var calculationResult = await _engine.CalculatePremiumAsync(model, await LoadConfigurationEntityAsync(configResult.Data.Id));
        
        if (!calculationResult.IsSuccess)
        {
            throw new InvalidOperationException($"Premium calculation failed: {calculationResult.Error}");
        }

        return calculationResult.Data;
    }

    public async Task<PremiumCalculationResultModel> CalculateEndorsementPremium(
        EndorsementViewModel endorsementVM, 
        PremiumCalculationResultModel premiumCalculation)
    {
        // For endorsements, we typically adjust the existing premium
        // This is a simplified implementation - can be extended based on endorsement rules
        
        if (string.IsNullOrEmpty(endorsementVM.PortfolioAlias))
        {
            throw new ArgumentException("PortfolioAlias is required", nameof(endorsementVM));
        }

        if (string.IsNullOrEmpty(endorsementVM.FiscalYear))
        {
            throw new ArgumentException("FiscalYear is required", nameof(endorsementVM));
        }

        // Get configuration
        var configResult = await _configService.GetConfigurationAsync(
            endorsementVM.PortfolioAlias, 
            endorsementVM.FiscalYear, 
            endorsementVM.EffectiveDate);

        if (!configResult.IsSuccess || configResult.Data == null)
        {
            throw new InvalidOperationException(
                $"Configuration not found for Portfolio: {endorsementVM.PortfolioAlias}, FiscalYear: {endorsementVM.FiscalYear}");
        }

        // Apply endorsement adjustments
        // This is a simplified version - can be extended with specific endorsement rules
        var adjustedCalculation = premiumCalculation;
        
        // Apply any endorsement-specific rules if configured
        var configEntity = await LoadConfigurationEntityAsync(configResult.Data.Id);
        var endorsementRules = configEntity.Rules
            .Where(r => r.IsActive && 
                       !r.IsDeleted && 
                       r.RuleType.ToString().Equals("Endorsement", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Priority)
            .ToList();

        if (endorsementRules.Any())
        {
            // EndorsementViewModel inherits from CreatePolicyViewModel, so we can use it directly
            var ruleResult = await _engine.ApplyRulesAsync(adjustedCalculation, endorsementRules, endorsementVM);
            if (ruleResult.IsSuccess)
            {
                adjustedCalculation = ruleResult.Data;
            }
        }

        return adjustedCalculation;
    }

    private async Task<PremiumCalculationConfiguration> LoadConfigurationEntityAsync(string id)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (config == null)
        {
            throw new InvalidOperationException($"Configuration with ID {id} not found");
        }

        return config;
    }
}

