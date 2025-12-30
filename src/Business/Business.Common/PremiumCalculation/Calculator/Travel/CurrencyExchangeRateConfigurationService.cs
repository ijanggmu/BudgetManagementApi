using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Common.Helper;
using Data.Context;
using Data.Entities.Common;
using Microsoft.AspNetCore.Http;
using Models.Common.Policy.Configuration.CurrencyExchangeRateConfiguration;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public class CurrencyExchangeRateConfigurationService : ICurrencyExchangeRateConfigurationService
{
    private ApplicationDataContext _db;
    private int page;
    private int perPage;

    public CurrencyExchangeRateConfigurationService(ApplicationDataContext db)
    {
        _db = db;
        page = 1;
        perPage = 100;
    }

    public async Task CreateCurrencyExchangeRateConfiguration(List<CurrencyExchangeRateConfigurationViewModel> model)
    {
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                var currencyExchangeRateConfigurationList = new List<CurrencyExchangeRateConfiguration>();
                _db.CurrencyExchangeRateConfigurations.RemoveRange(_db.CurrencyExchangeRateConfigurations.ToList());
                foreach (var item in model)
                {
                    currencyExchangeRateConfigurationList.Add(new CurrencyExchangeRateConfiguration
                    {
                        BaseCurrency = item.BaseCurrency,
                        BaseValue = item.BaseValue ?? 0,
                        Date = DateTime.UtcNow,
                        TargetBuy = item.TargetBuy ?? 0,
                        TargetCurrency = item.TargetCurrency,
                        TargetSell = item.TargetSell ?? 0,
                        CreatedOn = DateTime.UtcNow,
                        LastModifiedOn = DateTime.UtcNow,
                    });
                }
                currencyExchangeRateConfigurationList.Add(new CurrencyExchangeRateConfiguration
                {
                    Date = DateTime.UtcNow,
                    BaseCurrency = "NPR",
                    TargetCurrency = "NPR",
                    BaseValue = 1,
                    TargetBuy = 1,
                    TargetSell = 1,
                    //CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                    //UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                    //CreatedDate = DateTime.UtcNow,
                    //UpdatedDate = DateTime.UtcNow
                });
                var task1 = _db.CurrencyExchangeRateConfigurations.AddRangeAsync(currencyExchangeRateConfigurationList);
                await Task.WhenAll(task1);
                transaction.Commit();
                await _db.SaveChangesAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public List<CurrencyExchangeRateConfigurationViewModel> GetAllCurrencyExchangeRateConfigurations()
    {
        return _db.CurrencyExchangeRateConfigurations.Select(x => new CurrencyExchangeRateConfigurationViewModel
        {
            Id = x.Id,
            BaseCurrency = x.BaseCurrency,
            BaseValue = x.BaseValue,
            TargetBuy = x.TargetBuy,
            TargetCurrency = x.TargetCurrency,
            TargetSell = x.TargetSell,
            Date = x.Date.ToShortDateString(),
            IsApproved = x.IsApproved
        }).ToList();
    }

    public CurrencyExchangeRateConfigurationViewModel GetLatestCurrencyExchangeRate(
        string baseCurrency, string targetCurrency)
    {
        var exchangeRateConfig = _db.CurrencyExchangeRateConfigurations.FirstOrDefault(
            f => f.BaseCurrency == baseCurrency && f.TargetCurrency == targetCurrency);

        if (exchangeRateConfig == null)
        {
            return null;
        }

        return new CurrencyExchangeRateConfigurationViewModel
        {
            Id = exchangeRateConfig.Id,
            BaseCurrency = exchangeRateConfig.BaseCurrency,
            BaseValue = exchangeRateConfig.BaseValue,
            TargetBuy = exchangeRateConfig.TargetBuy,
            TargetCurrency = exchangeRateConfig.TargetCurrency,
            TargetSell = exchangeRateConfig.TargetSell,
            Date = exchangeRateConfig.Date.ToShortDateString(),
            IsApproved = exchangeRateConfig.IsApproved
        };
    }

    //public async Task<string> GetAllCurrencyExchangeRateConfigurationsFromExternalApiFromIssueDate(DateTime issueDate, string baseCurrency, string targetCurrency)
    //{
    //    using (var client = new HttpClient())
    //    {
    //        string today = issueDate.ConvertToLocalDateTime("yyyy-MM-dd");
    //        HttpResponseMessage response = await client.GetAsync($"{OrganizationDataConstants.ForeginExchangeUrl}?page={page}&per_page={perPage}&from={today}&to={today}");
    //        if (response.IsSuccessStatusCode)
    //        {
    //            var data = await response.Content.ReadAsAsync<CoversionJsonString>();
    //            var currencyInfoByCur = data.Data.Payload.FirstOrDefault().Rates.Where(x => x.Currency.ISO3 == baseCurrency).First();
    //            var targetSell = currencyInfoByCur.Sell;
    //            return targetSell;
    //        }
    //        return null;
    //    }
    //}

    //public async Task<List<CurrencyExchangeRateConfigurationViewModel>> GetAllCurrencyExchangeRateConfigurationsFromExternalApiFromSchedular()
    //{

    //    string today = DateTime.UtcNow.ConvertToLocalDateTime("yyyy-MM-dd");
    //    int page = 1;
    //    int perPage = 100;
    //    HttpResponseMessage response = await _httpClient.GetAsync($"{OrganizationDataConstants.ForeginExchangeUrl}?page={page}&per_page={perPage}&from={today}&to={today}");
    //    if (response.IsSuccessStatusCode)
    //    {
    //        var data = await response.Content.ReadAsAsync<CoversionJsonString>();
    //        Payload payload = data.Data.Payload.FirstOrDefault();
    //        string date = Convert.ToDateTime(payload.Date).ToString("MM/dd/yyyy");
    //        return payload.Rates.Select(x => new CurrencyExchangeRateConfigurationViewModel
    //        {
    //            Date = date,
    //            BaseCurrency = x.Currency.ISO3,
    //            BaseValue = x.Currency.Unit,
    //            TargetCurrency = "NPR",
    //            TargetBuy = Convert.ToDecimal(x.Buy),
    //            TargetSell = Convert.ToDecimal(x.Sell)
    //        })
    //            .ToList();

    //    }
    //    return null;


    //}


    public List<string> GetAllCurrencyNames()
    {
        return _db.CurrencyExchangeRateConfigurations.Select(x => x.BaseCurrency).ToList();
    }
}
