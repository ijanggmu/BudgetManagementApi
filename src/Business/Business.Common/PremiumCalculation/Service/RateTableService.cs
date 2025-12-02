using System.Text.Json;
using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Service;

public class RateTableService : IRateTableService
{
    private readonly ApplicationDataContext _db;

    public RateTableService(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task<Result<decimal>> GetRateAsync(
        string configurationId, 
        string tableName, 
        string lookupKey, 
        object lookupValue)
    {
        var rateTable = await _db.PremiumCalculationRateTables
            .Where(t => t.ConfigurationId == configurationId && 
                       t.TableName == tableName && 
                       !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (rateTable == null)
        {
            return Result<decimal>.Failed($"Rate table '{tableName}' not found");
        }

        try
        {
            var data = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(rateTable.DataJson);
            if (data == null || !data.Any())
            {
                return Result<decimal>.Failed("Rate table data is empty");
            }

            var lookupValueStr = lookupValue?.ToString()?.ToLowerInvariant();
            var row = data.FirstOrDefault(r => 
                r.ContainsKey(rateTable.LookupKey) && 
                r[rateTable.LookupKey].GetString()?.ToLowerInvariant() == lookupValueStr);

            if (row == null)
            {
                return Result<decimal>.Failed($"No matching row found for lookup value '{lookupValue}'");
            }

            // Find the rate column (typically "Rate" or similar)
            var rateColumn = row.Keys.FirstOrDefault(k => 
                k.Contains("Rate", StringComparison.OrdinalIgnoreCase) || 
                k.Contains("Premium", StringComparison.OrdinalIgnoreCase));

            if (rateColumn == null)
            {
                return Result<decimal>.Failed("Rate column not found in table");
            }

            var rate = row[rateColumn].GetDecimal();
            return Result<decimal>.Success(rate);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Failed($"Error reading rate table: {ex.Message}");
        }
    }

    public async Task<Result<decimal?>> GetRateRangeAsync(
        string configurationId, 
        string tableName, 
        string lookupKey, 
        object lookupValue, 
        string rangeColumn)
    {
        var rateTable = await _db.PremiumCalculationRateTables
            .Where(t => t.ConfigurationId == configurationId && 
                       t.TableName == tableName && 
                       !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (rateTable == null)
        {
            return Result<decimal?>.Failed($"Rate table '{tableName}' not found");
        }

        try
        {
            var data = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(rateTable.DataJson);
            if (data == null || !data.Any())
            {
                return Result<decimal?>.Failed("Rate table data is empty");
            }

            var lookupValueDecimal = Convert.ToDecimal(lookupValue);
            
            // Find row where lookup value falls within range
            var row = data.FirstOrDefault(r =>
            {
                if (!r.ContainsKey(rateTable.LookupKey) || !r.ContainsKey(rangeColumn))
                    return false;

                var minValue = r[rateTable.LookupKey].GetDecimal();
                var maxValue = r.ContainsKey("Max" + rateTable.LookupKey) 
                    ? r["Max" + rateTable.LookupKey].GetDecimal() 
                    : minValue;

                return lookupValueDecimal >= minValue && lookupValueDecimal <= maxValue;
            });

            if (row == null)
            {
                return Result<decimal?>.Success(null); // No matching range
            }

            var rateColumn = row.Keys.FirstOrDefault(k => 
                k.Contains("Rate", StringComparison.OrdinalIgnoreCase) || 
                k.Contains("Premium", StringComparison.OrdinalIgnoreCase));

            if (rateColumn == null)
            {
                return Result<decimal?>.Failed("Rate column not found in table");
            }

            var rate = row[rateColumn].GetDecimal();
            return Result<decimal?>.Success(rate);
        }
        catch (Exception ex)
        {
            return Result<decimal?>.Failed($"Error reading rate table: {ex.Message}");
        }
    }
}

