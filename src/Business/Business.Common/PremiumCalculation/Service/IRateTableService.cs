using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Service;

public interface IRateTableService
{
    Task<Result<decimal>> GetRateAsync(string configurationId, string tableName, string lookupKey, object lookupValue);
    Task<Result<decimal?>> GetRateRangeAsync(string configurationId, string tableName, string lookupKey, object lookupValue, string rangeColumn);
}

