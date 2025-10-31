using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Configuration.CurrencyExchangeRateConfiguration
{
    public class CoversionJsonString {
        public Conversion Data { get; set; }
    }

    public class Conversion {
        public Payload[] Payload { get; set; }
    }

    public class Payload
    {
        public string Date { get; set; }
        public Rates[] Rates { get; set; }
    }
    public class Rates
    {
        public CurrencyDetail Currency { get; set; }
        public string Buy { get; set; }
        public string Sell { get; set; }
    }
    public class CurrencyDetail
    {
        public string ISO3 { get; set; }
        public string Name { get; set; }
        public decimal Unit { get; set; }
    }
    public class Currency
    {
        public string Date { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public string BaseValue { get; set; }
        public string TargetBuy { get; set; }
        public string TargetSell { get; set; }

    }
    public class CurrencyExchangeRateConfigurationViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public bool IsApproved { get; set; }
        public string Date { get; set; }
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = ("Must be Non Space Capital Alphatbets only"))]
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public decimal? BaseValue { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public decimal? TargetBuy { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public decimal? TargetSell { get; set; }
    }

    public class CurrencyExchangeRateViewModel{
      public List<CurrencyExchangeRateConfigurationViewModel> ConfigurationList { get; set; }
      public CurrencyExchangeRateConfigurationViewModel ConfigurationViewModel { get; set; }
    }
}
