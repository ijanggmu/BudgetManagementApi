using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.Calculation;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;

namespace Business.Common.Helper;
public static class CalculationConfigMapper
{
    public static List<CalculationConfigurationViewModel> MapToCalculationConfigurationViewModel(List<CalculationConfiguration> entities)
    {
        if (entities == null || entities.Count == 0)
            return new List<CalculationConfigurationViewModel>();

        return entities.Select(entity => new CalculationConfigurationViewModel
        {

            Id = entity.Id,

            PortfolioAlias = entity.PortfolioAlias,
            PortfolioName = entity.PortfolioName,

            Type = entity.Type,
            TypeEnumValue = entity.TypeEnumValue,

            DataType = entity.DataType,
            ValueType = entity.ValueType,

            Value = entity.Value,
            Level = entity.Level,

            LowerLimit = entity.LowerLimit,
            UpperLimit = entity.UpperLimit,
            LowerLimitEquals = entity.LowerLimitEquals,
            UpperLimitEquals = entity.UpperLimitEquals,

            IsConfigured = entity.IsConfigured,

            CreatedDate = entity.CreatedOn.ToString("yyyy-MM-dd"),

            IsDeleted = entity.IsDeleted,

            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.LastModifiedBy
        })
        .ToList();
    }
    public static List<GlobalConfigurationViewModel> MapToGlobalConfigurationViewModel(List<GlobalConfiguration> entities)
    {
        if (entities == null || entities.Count == 0)
            return new List<GlobalConfigurationViewModel>();

        return entities
        .Select((entity, index) => new GlobalConfigurationViewModel
        {
            SN = index + 1,

            Id = entity.Id,

            Type = entity.Type,
            TypeEnumValue = entity.TypeEnumValue,

            DataType = entity.DataType,
            ValueType = entity.ValueType,

            Value = entity.Value,
            Level = entity.Level,

            LowerLimit = entity.LowerLimit,
            UpperLimit = entity.UpperLimit,
            LowerLimitEquals = entity.LowerLimitEquals,
            UpperLimitEquals = entity.UpperLimitEquals,

            IsConfigured = entity.IsConfigured,

            CreatedDate = entity.CreatedOn.ToString("yyyy-MM-dd"),

            IsDeleted = entity.IsDeleted,

            TypeName = entity.Type,

            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.LastModifiedBy
        })
        .ToList();

    }
}
