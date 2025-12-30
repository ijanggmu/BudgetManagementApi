using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public class TravelRateService : ITravelRateService
{
    private readonly ApplicationDataContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TravelRateService(
        ApplicationDataContext db,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Generate USD Travel Rates

    public async Task<bool> GenerateTravelUSDRateAsync(TravelRateRootobject request)
    {
        if (request?.TravelRateList == null || !request.TravelRateList.Any())
            return false;

        if (await _db.TravelUSDRates.AnyAsync())
            return false;

        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var createdBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "SYSTEM";
            var now = DateTime.UtcNow;

            var rates = request.TravelRateList.Select(x => new TravelUSDRate
            {
                AgeFrom = ParseInt(x.AgeFrom),
                AgeTo = ParseInt(x.AgeTo),
                PeriodFrom = ParseInt(x.PeriodFrom),
                PeriodTo = ParseInt(x.PeriodTo),

                Issuer = x.Issuer?.Trim(),
                DestintionIncludes = x.DestintionIncludes?.Trim(),
                PlanType = x.PlanType?.Trim(),
                Group = x.Group?.Trim(),
                MultipleEntries = x.MultipleEntries?.Trim(),

                IndividaulRate = ParseDecimal(x.IndividaulRate),
                FamilyRate = ParseDecimal(x.FamilyRate),

                CreatedBy = createdBy,
                CreatedOn = now,
                IsDeleted = false
            }).ToList();

            await _db.TravelUSDRates.AddRangeAsync(rates);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion

    #region Get USD Travel Rate

    public async Task<decimal> GetTravelUSDRateAsync(TravelUSDRateRequestModel request)
    {
        if (request == null)
            return 0;

        NormalizeMultipleEntryRule(request);

        var rate = await _db.TravelUSDRates
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.Issuer == request.Issuer &&
                x.DestintionIncludes == request.DestinationIncludes &&
                x.PlanType == request.PlanType &&
                x.Group == request.Group &&
                x.MultipleEntries == request.MultipleEntries &&
                x.AgeFrom <= request.Age && x.AgeTo >= request.Age &&
                x.PeriodFrom <= request.Period && x.PeriodTo >= request.Period)
            .FirstOrDefaultAsync();

        if (rate == null)
            return 0;

        return request.IsIndividual
            ? rate.IndividaulRate
            : rate.FamilyRate;
    }

    #endregion

    #region Get HEOMI Travel Rate

    public async Task<decimal> GetTravelHEOMIRateAsync(HEOMIRateRequestModel request)
    {
        if (request == null)
            return 0;

        var rate = await _db.HEOMITravelRates
            .AsNoTracking()
            .Where(x =>
                x.Plan.ToLower() == request.PlanType.ToLower() &&
                x.IsAnnualTrip == request.IsAnnualTrip &&
                x.PeriodFrom <= request.Period &&
                x.PeriodTo >= request.Period)
            .FirstOrDefaultAsync();

        if (rate == null)
            return 0;

        return request.IsIndividual
            ? Convert.ToDecimal(rate.IndividualRate)
            : Convert.ToDecimal(rate.FamilyRate);
    }

    #endregion

    #region Private Helpers

    private static void NormalizeMultipleEntryRule(TravelUSDRateRequestModel request)
    {
        if (request.Issuer is "JB" or "ITI")
        {
            request.MultipleEntries =
                request.Period is >= 181 and <= 365 ? "yes" :
                request.Issuer == "ITI" ? "false" : "no";
        }
    }

    private static int ParseInt(string value)
        => int.TryParse(value, out var result) ? result : 0;

    private static decimal ParseDecimal(string value)
        => decimal.TryParse(value, out var result) ? result : 0;

    #endregion
}

