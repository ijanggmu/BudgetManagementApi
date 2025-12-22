using System.Net;
using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public class PremiumCalculationRuleService : IPremiumCalculationRuleService
{
    private readonly ApplicationDataContext _db;
    private readonly IPremiumCalculationConfigurationService _configService;
    private readonly IUserProfileService _userProfileService;

    public PremiumCalculationRuleService(
        ApplicationDataContext db,
        IPremiumCalculationConfigurationService configService,
        IUserProfileService userProfileService)
    {
        _db = db;
        _configService = configService;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<PremiumCalculationRuleDto>>> GetRulesByConfigurationIdAsync(string configurationId, CancellationToken cancellationToken = default)
    {
        var config = await _configService.GetConfigurationByIdAsync(configurationId, cancellationToken);
        if (!config.IsSuccess)
        {
            return Result<List<PremiumCalculationRuleDto>>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        return Result<List<PremiumCalculationRuleDto>>.Success(config.Data.Rules);
    }

    public async Task<Result<PremiumCalculationRuleDto>> GetRuleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var rule = await _db.PremiumCalculationRules
            .Where(r => r.Id == id && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rule == null)
        {
            return Result<PremiumCalculationRuleDto>.Failed(
                "Rule not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        return Result<PremiumCalculationRuleDto>.Success(MapToDto(rule));
    }

    public async Task<Result<PremiumCalculationRuleDto>> CreateRuleAsync(
        string configurationId, 
        CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default)
    {
        // Verify configuration exists
        var config = await _configService.GetConfigurationByIdAsync(configurationId, cancellationToken);
        if (!config.IsSuccess)
        {
            return Result<PremiumCalculationRuleDto>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        var rule = new PremiumCalculationRule
        {
            ConfigurationId = configurationId,
            RuleName = dto.RuleName,
            RuleType = dto.RuleType,
            Expression = dto.Expression,
            Condition = dto.Condition,
            Priority = dto.Priority,
            IsActive = dto.IsActive
        };

        _db.PremiumCalculationRules.Add(rule);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationRuleDto>.Success(MapToDto(rule));
    }

    public async Task<Result<PremiumCalculationRuleDto>> UpdateRuleAsync(
        string id, 
        CreatePremiumCalculationRuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = await _db.PremiumCalculationRules
            .Where(r => r.Id == id && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rule == null)
        {
            return Result<PremiumCalculationRuleDto>.Failed(
                "Rule not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        rule.RuleName = dto.RuleName;
        rule.RuleType = dto.RuleType;
        rule.Expression = dto.Expression;
        rule.Condition = dto.Condition;
        rule.Priority = dto.Priority;
        rule.IsActive = dto.IsActive;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationRuleDto>.Success(MapToDto(rule));
    }

    public async Task<Result<bool>> DeleteRuleAsync(string id, CancellationToken cancellationToken = default)
    {
        var rule = await _db.PremiumCalculationRules
            .Where(r => r.Id == id && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rule == null)
        {
            return Result<bool>.Failed(
                "Rule not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        rule.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static PremiumCalculationRuleDto MapToDto(PremiumCalculationRule rule)
    {
        return new PremiumCalculationRuleDto(
            rule.Id,
            rule.ConfigurationId,
            rule.RuleName,
            rule.RuleType,
            rule.Expression,
            rule.Condition,
            rule.Priority,
            rule.IsActive
        );
    }
}

