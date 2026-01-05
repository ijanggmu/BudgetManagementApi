using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;

namespace Data.Entities.Common;
public class CurrencyExchangeRateConfiguration : ApplicationBaseEntity
{
    public DateTime Date { get; set; }
    public string BaseCurrency { get; set; }
    public string TargetCurrency { get; set; }
    public decimal BaseValue { get; set; }
    public decimal TargetBuy { get; set; }
    public decimal TargetSell { get; set; }
    public bool IsApproved { get; set; }
}
