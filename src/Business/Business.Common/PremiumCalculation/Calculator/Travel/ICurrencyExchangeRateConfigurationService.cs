using Models.Common.Policy.Configuration.CurrencyExchangeRateConfiguration;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public interface ICurrencyExchangeRateConfigurationService
{
    Task CreateCurrencyExchangeRateConfiguration(List<CurrencyExchangeRateConfigurationViewModel> model);
    List<CurrencyExchangeRateConfigurationViewModel> GetAllCurrencyExchangeRateConfigurations();
    List<string> GetAllCurrencyNames();
    CurrencyExchangeRateConfigurationViewModel GetLatestCurrencyExchangeRate(string baseCurrency, string targetCurrency);
}