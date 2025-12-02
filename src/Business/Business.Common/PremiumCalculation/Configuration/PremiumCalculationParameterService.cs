using System.Net;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public class PremiumCalculationParameterService : IPremiumCalculationParameterService
{
    private readonly ApplicationDataContext _db;
    private readonly IPremiumCalculationConfigurationService _configService;
    private readonly IUserProfileService _userProfileService;

    public PremiumCalculationParameterService(
        ApplicationDataContext db,
        IPremiumCalculationConfigurationService configService,
        IUserProfileService userProfileService)
    {
        _db = db;
        _configService = configService;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<PremiumCalculationParameterDto>>> GetParametersByConfigurationIdAsync(string configurationId)
    {
        var config = await _configService.GetConfigurationByIdAsync(configurationId);
        if (!config.IsSuccess)
        {
            return Result<List<PremiumCalculationParameterDto>>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        return Result<List<PremiumCalculationParameterDto>>.Success(config.Data.Parameters);
    }

    public async Task<Result<PremiumCalculationParameterDto>> GetParameterByIdAsync(string id)
    {
        var parameter = await _db.PremiumCalculationParameters
            .Where(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync();

        if (parameter == null)
        {
            return Result<PremiumCalculationParameterDto>.Failed(
                "Parameter not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        return Result<PremiumCalculationParameterDto>.Success(MapToDto(parameter));
    }

    public async Task<Result<PremiumCalculationParameterDto>> CreateParameterAsync(
        string configurationId, 
        CreatePremiumCalculationParameterDto dto)
    {
        // Verify configuration exists
        var config = await _configService.GetConfigurationByIdAsync(configurationId);
        if (!config.IsSuccess)
        {
            return Result<PremiumCalculationParameterDto>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        // Check if parameter key already exists
        var exists = await _db.PremiumCalculationParameters
            .AnyAsync(p => p.ConfigurationId == configurationId && 
                          p.ParameterKey == dto.ParameterKey && 
                          !p.IsDeleted);

        if (exists)
        {
            return Result<PremiumCalculationParameterDto>.Failed(
                "Parameter with this key already exists for this configuration");
        }

        var parameter = new PremiumCalculationParameter
        {
            ConfigurationId = configurationId,
            ParameterKey = dto.ParameterKey,
            ParameterName = dto.ParameterName,
            DataType = dto.DataType,
            Value = dto.Value,
            DefaultValue = dto.DefaultValue,
            MinValue = dto.MinValue,
            MaxValue = dto.MaxValue,
            IsRequired = dto.IsRequired,
            DisplayOrder = dto.DisplayOrder,
            Category = dto.Category
        };

        _db.PremiumCalculationParameters.Add(parameter);
        await _db.SaveChangesAsync();

        return Result<PremiumCalculationParameterDto>.Success(MapToDto(parameter));
    }

    public async Task<Result<PremiumCalculationParameterDto>> UpdateParameterAsync(
        string id, 
        CreatePremiumCalculationParameterDto dto)
    {
        var parameter = await _db.PremiumCalculationParameters
            .Where(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync();

        if (parameter == null)
        {
            return Result<PremiumCalculationParameterDto>.Failed(
                "Parameter not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        parameter.ParameterName = dto.ParameterName;
        parameter.DataType = dto.DataType;
        parameter.Value = dto.Value;
        parameter.DefaultValue = dto.DefaultValue;
        parameter.MinValue = dto.MinValue;
        parameter.MaxValue = dto.MaxValue;
        parameter.IsRequired = dto.IsRequired;
        parameter.DisplayOrder = dto.DisplayOrder;
        parameter.Category = dto.Category;

        await _db.SaveChangesAsync();

        return Result<PremiumCalculationParameterDto>.Success(MapToDto(parameter));
    }

    public async Task<Result<bool>> DeleteParameterAsync(string id)
    {
        var parameter = await _db.PremiumCalculationParameters
            .Where(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync();

        if (parameter == null)
        {
            return Result<bool>.Failed(
                "Parameter not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        parameter.IsDeleted = true;
        await _db.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> BulkUpdateParametersAsync(
        string configurationId, 
        List<CreatePremiumCalculationParameterDto> parameters)
    {
        // Verify configuration exists
        var config = await _configService.GetConfigurationByIdAsync(configurationId);
        if (!config.IsSuccess)
        {
            return Result<bool>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        // Delete existing parameters (soft delete)
        await _db.PremiumCalculationParameters
            .Where(p => p.ConfigurationId == configurationId && !p.IsDeleted)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.IsDeleted, true));

        // Add new parameters
        foreach (var dto in parameters)
        {
            var parameter = new PremiumCalculationParameter
            {
                ConfigurationId = configurationId,
                ParameterKey = dto.ParameterKey,
                ParameterName = dto.ParameterName,
                DataType = dto.DataType,
                Value = dto.Value,
                DefaultValue = dto.DefaultValue,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                IsRequired = dto.IsRequired,
                DisplayOrder = dto.DisplayOrder,
                Category = dto.Category
            };

            _db.PremiumCalculationParameters.Add(parameter);
        }

        await _db.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static PremiumCalculationParameterDto MapToDto(PremiumCalculationParameter parameter)
    {
        return new PremiumCalculationParameterDto(
            parameter.Id,
            parameter.ConfigurationId,
            parameter.ParameterKey,
            parameter.ParameterName,
            parameter.DataType,
            parameter.Value,
            parameter.DefaultValue,
            parameter.MinValue,
            parameter.MaxValue,
            parameter.IsRequired,
            parameter.DisplayOrder,
            parameter.Category
        );
    }
}

