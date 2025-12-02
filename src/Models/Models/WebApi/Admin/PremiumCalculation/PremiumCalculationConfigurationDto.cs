using System;
using System.Collections.Generic;

namespace Models.WebApi.Admin.PremiumCalculation;

public record PremiumCalculationConfigurationDto(
    string Id,
    string PortfolioAlias,
    string FiscalYear,
    int Version,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsActive,
    string? Description,
    string CalculationEngineType,
    DateTime CreatedOn,
    DateTime? LastModifiedOn,
    List<PremiumCalculationParameterDto> Parameters,
    List<PremiumCalculationRuleDto> Rules,
    List<PremiumCalculationRateTableDto> RateTables
);

public record PremiumCalculationParameterDto(
    string Id,
    string ConfigurationId,
    string ParameterKey,
    string ParameterName,
    string DataType,
    string Value,
    string? DefaultValue,
    decimal? MinValue,
    decimal? MaxValue,
    bool IsRequired,
    int DisplayOrder,
    string? Category
);

public record PremiumCalculationRuleDto(
    string Id,
    string ConfigurationId,
    string RuleName,
    string RuleType,
    string? Expression,
    string? Condition,
    int Priority,
    bool IsActive
);

public record PremiumCalculationRateTableDto(
    string Id,
    string ConfigurationId,
    string TableName,
    string SchemaJson,
    string DataJson,
    string LookupKey
);

