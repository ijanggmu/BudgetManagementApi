using System.Net;
using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public class PremiumCalculationConfigurationService : IPremiumCalculationConfigurationService
{
    private readonly ApplicationDataContext _db;
    private readonly IUserProfileService _userProfileService;

    public PremiumCalculationConfigurationService(
        ApplicationDataContext db,
        IUserProfileService userProfileService)
    {
        _db = db;
        _userProfileService = userProfileService;
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationAsync(
        string portfolioAlias, 
        string fiscalYear, 
        DateTime? effectiveDate = null, CancellationToken cancellationToken = default)
    {
        var query = _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.PortfolioAlias == portfolioAlias && 
                       c.FiscalYear == fiscalYear && 
                       c.IsActive &&
                       !c.IsDeleted);

        if (effectiveDate.HasValue)
        {
            var date = DateOnly.FromDateTime(effectiveDate.Value);
            query = query.Where(c => c.EffectiveFrom <= date && 
                                   (c.EffectiveTo == null || c.EffectiveTo >= date));
        }

        var config = await query
            .OrderByDescending(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration not found for the specified portfolio and fiscal year",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<List<PremiumCalculationConfigurationDto>>> GetAllConfigurationsAsync(
        string? portfolioAlias = null, 
        string? fiscalYear = null, CancellationToken cancellationToken = default)
    {
        var query = _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(portfolioAlias))
        {
            query = query.Where(c => c.PortfolioAlias == portfolioAlias);
        }

        if (!string.IsNullOrEmpty(fiscalYear))
        {
            query = query.Where(c => c.FiscalYear == fiscalYear);
        }

        var configs = await query
            .OrderByDescending(c => c.FiscalYear)
            .ThenByDescending(c => c.Version)
            .ToListAsync(cancellationToken);

        var dtos = configs.Select(MapToDto).ToList();
        return Result<List<PremiumCalculationConfigurationDto>>.Success(dtos);
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> CreateConfigurationAsync(
        CreatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default)
    {
        // Check if configuration already exists
        var exists = await _db.PremiumCalculationConfigurations
            .AnyAsync(c => c.PortfolioAlias == dto.PortfolioAlias && 
                          c.FiscalYear == dto.FiscalYear && 
                          !c.IsDeleted, cancellationToken);

        if (exists)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration already exists for this portfolio and fiscal year");
        }

        var config = new PremiumCalculationConfiguration
        {
            PortfolioAlias = dto.PortfolioAlias,
            FiscalYear = dto.FiscalYear,
            Version = 1,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true,
            Description = dto.Description,
            CalculationEngineType = dto.CalculationEngineType
        };

        _db.PremiumCalculationConfigurations.Add(config);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> UpdateConfigurationAsync(
        string id, 
        UpdatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        if (dto.EffectiveFrom.HasValue)
            config.EffectiveFrom = dto.EffectiveFrom.Value;
        if (dto.EffectiveTo.HasValue)
            config.EffectiveTo = dto.EffectiveTo;
        if (dto.IsActive.HasValue)
            config.IsActive = dto.IsActive.Value;
        if (!string.IsNullOrEmpty(dto.Description))
            config.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.CalculationEngineType))
            config.CalculationEngineType = dto.CalculationEngineType;

        await _db.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var updated = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(updated!));
    }

    public async Task<Result<bool>> DeleteConfigurationAsync(string id, CancellationToken cancellationToken = default)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<bool>.Failed(
                "Configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        config.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> ActivateConfigurationAsync(
        string id, 
        string fiscalYear, CancellationToken cancellationToken = default)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        // Deactivate all other configurations for the same portfolio and fiscal year
        await _db.PremiumCalculationConfigurations
            .Where(c => c.PortfolioAlias == config.PortfolioAlias && 
                       c.FiscalYear == fiscalYear && 
                       c.Id != id && 
                       !c.IsDeleted)
            .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.IsActive, false), cancellationToken);

        config.IsActive = true;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<PremiumCalculationConfigurationDto>> CloneConfigurationAsync(
        string id, 
        string newFiscalYear, CancellationToken cancellationToken = default)
    {
        var source = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (source == null)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Source configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        // Check if target already exists
        var exists = await _db.PremiumCalculationConfigurations
            .AnyAsync(c => c.PortfolioAlias == source.PortfolioAlias && 
                          c.FiscalYear == newFiscalYear && 
                          !c.IsDeleted, cancellationToken);

        if (exists)
        {
            return Result<PremiumCalculationConfigurationDto>.Failed(
                "Configuration already exists for the target fiscal year");
        }

        var cloned = new PremiumCalculationConfiguration
        {
            PortfolioAlias = source.PortfolioAlias,
            FiscalYear = newFiscalYear,
            Version = 1,
            EffectiveFrom = source.EffectiveFrom,
            EffectiveTo = source.EffectiveTo,
            IsActive = false, // Inactive by default
            Description = source.Description,
            CalculationEngineType = source.CalculationEngineType
        };

        _db.PremiumCalculationConfigurations.Add(cloned);
        await _db.SaveChangesAsync(cancellationToken);

        // Clone parameters
        foreach (var param in source.Parameters.Where(p => !p.IsDeleted))
        {
            _db.PremiumCalculationParameters.Add(new PremiumCalculationParameter
            {
                ConfigurationId = cloned.Id,
                ParameterKey = param.ParameterKey,
                ParameterName = param.ParameterName,
                DataType = param.DataType,
                Value = param.Value,
                DefaultValue = param.DefaultValue,
                MinValue = param.MinValue,
                MaxValue = param.MaxValue,
                IsRequired = param.IsRequired,
                DisplayOrder = param.DisplayOrder,
                Category = param.Category
            });
        }

        // Clone rules
        foreach (var rule in source.Rules.Where(r => r.IsActive && !r.IsDeleted))
        {
            _db.PremiumCalculationRules.Add(new PremiumCalculationRule
            {
                ConfigurationId = cloned.Id,
                RuleName = rule.RuleName,
                RuleType = rule.RuleType,
                Expression = rule.Expression,
                Condition = rule.Condition,
                Priority = rule.Priority,
                IsActive = rule.IsActive
            });
        }

        // Clone rate tables
        foreach (var table in source.RateTables.Where(t => !t.IsDeleted))
        {
            _db.PremiumCalculationRateTables.Add(new PremiumCalculationRateTable
            {
                ConfigurationId = cloned.Id,
                TableName = table.TableName,
                SchemaJson = table.SchemaJson,
                DataJson = table.DataJson,
                LookupKey = table.LookupKey
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var result = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Include(c => c.RateTables)
            .Where(c => c.Id == cloned.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<PremiumCalculationConfigurationDto>.Success(MapToDto(result!));
    }

    public async Task<Result<object>> GetParameterValueAsync(
        string portfolioAlias, 
        string fiscalYear, 
        string parameterKey, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(portfolioAlias, fiscalYear, null, cancellationToken);
        if (!config.IsSuccess)
        {
            return Result<object>.Failed(config.Error, config.ErrorCode, null, config.StatusCode);
        }

        var parameter = config.Data.Parameters
            .FirstOrDefault(p => p.ParameterKey == parameterKey);

        if (parameter == null)
        {
            return Result<object>.Failed(
                $"Parameter '{parameterKey}' not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        // Deserialize based on data type
        object value = parameter.DataType.ToLower() switch
        {
            "decimal" => decimal.Parse(parameter.Value),
            "int" => int.Parse(parameter.Value),
            "bool" => bool.Parse(parameter.Value),
            "string" => parameter.Value,
            _ => parameter.Value
        };

        return Result<object>.Success(value);
    }

    public async Task<Result<bool>> ValidateConfigurationAsync(string configurationId, CancellationToken cancellationToken = default)
    {
        var config = await _db.PremiumCalculationConfigurations
            .Include(c => c.Parameters)
            .Include(c => c.Rules)
            .Where(c => c.Id == configurationId && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
        {
            return Result<bool>.Failed(
                "Configuration not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        // Basic validation
        var errors = new List<string>();

        // Check required parameters
        var requiredParams = config.Parameters
            .Where(p => p.IsRequired && string.IsNullOrEmpty(p.Value) && !p.IsDeleted)
            .ToList();

        if (requiredParams.Any())
        {
            errors.Add($"Missing required parameters: {string.Join(", ", requiredParams.Select(p => p.ParameterKey))}");
        }

        // Check date range
        if (config.EffectiveTo.HasValue && config.EffectiveFrom > config.EffectiveTo.Value)
        {
            errors.Add("EffectiveFrom date must be before EffectiveTo date");
        }

        if (errors.Any())
        {
            return Result<bool>.Failed(string.Join("; ", errors));
        }

        return Result<bool>.Success(true);
    }

    private static PremiumCalculationConfigurationDto MapToDto(PremiumCalculationConfiguration config)
    {
        return new PremiumCalculationConfigurationDto(
            config.Id,
            config.PortfolioAlias,
            config.FiscalYear,
            config.Version,
            config.EffectiveFrom,
            config.EffectiveTo,
            config.IsActive,
            config.Description,
            config.CalculationEngineType,
            config.CreatedOn,
            config.LastModifiedOn,
            config.Parameters.Where(p => !p.IsDeleted).Select(p => new PremiumCalculationParameterDto(
                p.Id,
                p.ConfigurationId,
                p.ParameterKey,
                p.ParameterName,
                p.DataType,
                p.Value,
                p.DefaultValue,
                p.MinValue,
                p.MaxValue,
                p.IsRequired,
                p.DisplayOrder,
                p.Category
            )).ToList(),
            config.Rules.Where(r => !r.IsDeleted).Select(r => new PremiumCalculationRuleDto(
                r.Id,
                r.ConfigurationId,
                r.RuleName,
                r.RuleType,
                r.Expression,
                r.Condition,
                r.Priority,
                r.IsActive
            )).ToList(),
            config.RateTables.Where(t => !t.IsDeleted).Select(t => new PremiumCalculationRateTableDto(
                t.Id,
                t.ConfigurationId,
                t.TableName,
                t.SchemaJson,
                t.DataJson,
                t.LookupKey
            )).ToList()
        );
    }
}

