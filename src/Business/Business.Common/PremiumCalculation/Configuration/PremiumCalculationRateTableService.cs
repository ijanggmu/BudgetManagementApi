using System.Net;
using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public class PremiumCalculationRateTableService : IPremiumCalculationRateTableService
{
    private readonly ApplicationDataContext _db;
    private readonly IPremiumCalculationConfigurationService _configService;
    private readonly IUserProfileService _userProfileService;

    public PremiumCalculationRateTableService(
        ApplicationDataContext db,
        IPremiumCalculationConfigurationService configService,
        IUserProfileService userProfileService)
    {
        _db = db;
        _configService = configService;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<PremiumCalculationRateTableDto>>> GetRateTablesByConfigurationIdAsync(string configurationId, CancellationToken cancellationToken = default)
    {
        var config = await _configService.GetConfigurationByIdAsync(configurationId, cancellationToken);
        if (!config.IsSuccess)
        {
            return Result<List<PremiumCalculationRateTableDto>>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        return Result<List<PremiumCalculationRateTableDto>>.Success(config.Data.RateTables);
    }

    public async Task<Result<PremiumCalculationRateTableDto>> GetRateTableByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var rateTable = await _db.PremiumCalculationRateTables
            .Where(t => t.Id == id && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rateTable == null)
        {
            return Result<PremiumCalculationRateTableDto>.Failed(
                "Rate table not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        return Result<PremiumCalculationRateTableDto>.Success(MapToDto(rateTable));
    }

    public async Task<Result<PremiumCalculationRateTableDto>> CreateRateTableAsync(
        string configurationId, 
        CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default)
    {
        // Verify configuration exists
        var config = await _configService.GetConfigurationByIdAsync(configurationId, cancellationToken);
        if (!config.IsSuccess)
        {
            return Result<PremiumCalculationRateTableDto>.Failed(
                config.Error, 
                config.ErrorCode, 
                null, 
                config.StatusCode);
        }

        // Check if rate table with same name already exists
        var exists = await _db.PremiumCalculationRateTables
            .AnyAsync(t => t.ConfigurationId == configurationId && 
                          t.TableName == dto.TableName && 
                          !t.IsDeleted, cancellationToken);

        if (exists)
        {
            return Result<PremiumCalculationRateTableDto>.Failed(
                "Rate table with this name already exists for this configuration");
        }

        var rateTable = new PremiumCalculationRateTable
        {
            ConfigurationId = configurationId,
            TableName = dto.TableName,
            SchemaJson = dto.SchemaJson,
            DataJson = dto.DataJson,
            LookupKey = dto.LookupKey
        };

        _db.PremiumCalculationRateTables.Add(rateTable);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationRateTableDto>.Success(MapToDto(rateTable));
    }

    public async Task<Result<PremiumCalculationRateTableDto>> UpdateRateTableAsync(
        string id, 
        CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default)
    {
        var rateTable = await _db.PremiumCalculationRateTables
            .Where(t => t.Id == id && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rateTable == null)
        {
            return Result<PremiumCalculationRateTableDto>.Failed(
                "Rate table not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        rateTable.TableName = dto.TableName;
        rateTable.SchemaJson = dto.SchemaJson;
        rateTable.DataJson = dto.DataJson;
        rateTable.LookupKey = dto.LookupKey;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<PremiumCalculationRateTableDto>.Success(MapToDto(rateTable));
    }

    public async Task<Result<bool>> DeleteRateTableAsync(string id, CancellationToken cancellationToken = default)
    {
        var rateTable = await _db.PremiumCalculationRateTables
            .Where(t => t.Id == id && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (rateTable == null)
        {
            return Result<bool>.Failed(
                "Rate table not found",
                (int)HttpStatusCode.NotFound,
                null,
                HttpStatusCode.NotFound);
        }

        rateTable.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static PremiumCalculationRateTableDto MapToDto(PremiumCalculationRateTable rateTable)
    {
        return new PremiumCalculationRateTableDto(
            rateTable.Id,
            rateTable.ConfigurationId,
            rateTable.TableName,
            rateTable.SchemaJson,
            rateTable.DataJson,
            rateTable.LookupKey
        );
    }
}

