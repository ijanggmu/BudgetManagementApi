using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy.Fire;

namespace Business.Common.PremiumCalculation.Service;
public static class FireRiskConfigurationStatic
{
    private static readonly List<FireRiskConfiguration> _configurations =
        new List<FireRiskConfiguration>
        {
            new FireRiskConfiguration
            {
                RiskCode = "1",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Residential Building or home, Temples, Meditation and Pray or Worship Place including Goods and Properties inside",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "2",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Brickworks of any Type",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "3",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Stone Quarries",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "4",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Granite",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "5",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Stone Crushing",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "6",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Stone Art",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "7",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Stoneware",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "8",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Sand",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "9",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Soil",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "10",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Marble",
                Rate = 0.10m
            },
            new FireRiskConfiguration
            {
                RiskCode = "11",
                RateCode = "1",
                RiskType = "अति सामान्य जोखिम",
                PropertyDescription = "Hollow Bricks",
                Rate = 0.10m
            }
        };

    public static List<FireRiskConfiguration> GetAll()
    {
        return _configurations.ToList();
    }
}
