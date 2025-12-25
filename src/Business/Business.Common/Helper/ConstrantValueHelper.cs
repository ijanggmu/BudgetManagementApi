using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;
using SharedKernel.Helper;

namespace Business.Common.Helper;
public class ConstrantValueHelper
{
    public static decimal GetValue(List<CalculationConfigurationViewModel> model, string key, decimal givenValue = 0)
    {
        try
        {
            decimal val = 0;
            var list = model.Where(x => x.Type == key).ToList();
            foreach (var item in list)
            {
                if (item.DataType.ToLower() == "discrete")
                {
                    if (item.ValueType.ToLower() == "rate")
                        val = item.Value / 100;
                    else
                        val = item.Value;
                    break;
                }
                else
                {
                    if (item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (item.UpperLimitEquals && !item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (!item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue < item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        if (givenValue < item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }
                    }
                }
            }
            return val;
        }
        catch
        {
            throw;
        }
    }

    public static decimal GetNewValue(List<CalculationConfigurationViewModel> model, int key, decimal givenValue = 0)
    {
        try

        {
            decimal val = 0;
            var list = model.Where(x => x.TypeEnumValue == key).ToList();
            foreach (var item in list)
            {
                if (item.DataType.ToLower() == "discrete")
                {
                    if (item.ValueType.ToLower() == "rate")
                        val = item.Value / 100;
                    else
                        val = item.Value;
                    break;
                }
                else if (item.DataType.ToLower() == "multidiscrete")
                {
                    var itm = list.FirstOrDefault(x => x.Value == givenValue);
                    if (itm.ValueType.ToLower() == "rate")
                        val = itm.Value / 100;
                    else
                        val = itm.Value;
                    break;
                }
                else
                {
                    if (item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (item.UpperLimitEquals && !item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (!item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue < item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        if (givenValue < item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }
                    }
                }
            }
            return val;
        }
        catch
        {
            throw;
        }
    }

    public static decimal GetGlobalValue(List<GlobalConfigurationViewModel> model, int key, decimal givenValue = 0)
    {
        try
        {
            decimal val = 0;
            var list = model.Where(x => x.TypeEnumValue == key).ToList();
            foreach (var item in list)
            {
                if (item.DataType.ToLower() == "discrete")
                {
                    if (item.ValueType.ToLower() == "rate")
                        val = item.Value / 100;
                    else
                        val = item.Value;
                    break;
                }
                else
                {
                    if (item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (item.UpperLimitEquals && !item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else if (!item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue < item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        if (givenValue < item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            if (item.ValueType.ToLower() == "rate")
                                val = item.Value / 100;
                            else
                                val = item.Value;
                            break;
                        }
                    }
                }
            }
            return val;
        }
        catch
        {
            throw;
        }
    }


    /// <summary>
    /// Returns policy-specific calculation configuration with Rate values as it is (without division by 100)
    /// </summary>
    /// <param name="model">Calculation configuration model to get values from.</param>
    /// <param name="key">The key for which the value is to be retrieved.</param>
    /// <param name="givenValue">The value of constrant that the specified key's value depends upon.</param>
    /// <returns></returns>
    public static decimal GetValueWithoutRateConversion(List<CalculationConfigurationViewModel> model, int key, decimal givenValue = 0, decimal level = 0)

    {
        try
        {
            decimal val = 0;
            var list = model.Where(x => x.TypeEnumValue == key).ToList();
            if (level > 0)
            {
                var levelData = list.Where(x => x.Level == level.RoundWithoutDecimalNepaliFormat().ToString()).ToList();
                if (!levelData.Any())
                {
                    val = 0;
                    return val;
                }
                else
                {
                    //todo manage code
                    foreach (var data in levelData)
                    {
                        if (data.UpperLimitEquals && data.LowerLimitEquals)
                        {
                            if (givenValue <= data.UpperLimit && givenValue >= data.LowerLimit)
                            {
                                val = data.Value;
                                break;
                            }
                            else { continue; }
                        }
                        else if (data.UpperLimitEquals && !data.LowerLimitEquals)
                        {
                            if (givenValue <= data.UpperLimit && givenValue > data.LowerLimit)
                            {
                                val = data.Value;
                                break;
                            }
                            else { continue; }
                        }
                        else if (!data.UpperLimitEquals && data.LowerLimitEquals)
                        {
                            if (givenValue < data.UpperLimit && givenValue >= data.LowerLimit)
                            {
                                val = data.Value;
                                break;
                            }
                            else { continue; }
                        }
                        else
                        {
                            if (givenValue < data.UpperLimit && givenValue > data.LowerLimit)
                            {
                                val = data.Value;
                                break;
                            }
                            else { continue; }
                        }

                    }
                    return val;
                }
            }
            foreach (var item in list)
            {
                if (item.DataType.ToLower() == "discrete")
                {
                    val = item.Value;
                    break;
                }
                else if (item.DataType.ToLower() == "multidiscrete")
                {
                    var itm = list.FirstOrDefault(x => x.Value == givenValue);
                    val = itm.Value;
                    break;
                }
                else
                {
                    if (item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else if (item.UpperLimitEquals && !item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else if (!item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue < item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        if (givenValue < item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }
                    }

                }

            }
            return val;
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Returns global calculation configuration with Rate values as it is (without division by 100)
    /// </summary>
    /// <param name="model">Calculation configuration model to get values from.</param>
    /// <param name="key">The key for which the value is to be retrieved.</param>
    /// <param name="givenValue">The value of constrant that the specified key's value depends upon.</param>
    public static decimal GetGlobalValueWithoutRateConversion(List<GlobalConfigurationViewModel> model, int key, decimal givenValue = 0)
    {
        try
        {
            decimal val = 0;
            var list = model.Where(x => x.TypeEnumValue == key).ToList();
            foreach (var item in list)
            {
                if (item.DataType.ToLower() == "discrete")
                {
                    val = item.Value;
                    break;
                }
                else
                {
                    if (item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else if (item.UpperLimitEquals && !item.LowerLimitEquals)
                    {
                        if (givenValue <= item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else if (!item.UpperLimitEquals && item.LowerLimitEquals)
                    {
                        if (givenValue < item.UpperLimit && givenValue >= item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        if (givenValue < item.UpperLimit && givenValue > item.LowerLimit)
                        {
                            val = item.Value;
                            break;
                        }
                    }
                }
            }
            return val;
        }
        catch
        {
            throw;
        }
    }

    public static List<CalculationConfigurationViewModel> GetMultipleValue(List<CalculationConfigurationViewModel> model, string key, decimal givenValue = 0)
    {
        var list = model?.Where(x => x.Type == key).ToList();
        if (list?.Count > 0)
            return list;
        else
            return null;
    }
}
