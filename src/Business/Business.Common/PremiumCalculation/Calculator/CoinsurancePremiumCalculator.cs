using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Common.PremiumCalculation.Calculator;
public static class CoinsurancePremiumCalculator
{
    public static decimal GetCoinsurancePremium(decimal source, decimal coinsuranceRate)
    {
        return source * coinsuranceRate / 100;
    }
}
