using System.Globalization;
using Models.Common.Policy.Enum;

namespace SharedKernel.Helper;
public static class DecimalHelper
{
    private static readonly CultureInfo _nepaliCulture = new CultureInfo("ne-NP");
    private static readonly CultureInfo _englishCulture = new CultureInfo("en-US");

    public static decimal RoundToFour(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 4, MidpointRounding.AwayFromZero);
    }
    public static decimal RoundToFourPrecisions(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 2);
    }
    public static decimal RoundToThree(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 3, MidpointRounding.AwayFromZero);
    }
    public static decimal RoundToSix(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 6);
    }
    public static decimal RoundToEight(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 8, MidpointRounding.AwayFromZero);
    }
    public static decimal RoundToTwo(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero);
    }
    public static string RoundToFourNepaliFormat(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 4, MidpointRounding.AwayFromZero).ToString("N4", new CultureInfo("en-IN"));
    }
    public static string RoundToThreeNepaliFormat(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, 3, MidpointRounding.AwayFromZero).ToString("N3", new CultureInfo("en-IN"));
    }
    public static string RoundToTwoNepaliFormat(this decimal decimalNumber) //10.0233
    {
        return Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero).ToString("N2", new CultureInfo("en-IN"));
    }
    public static string RoundToTwoDecimalFormat(this decimal decimalNumber)//10.02
    {
        return Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero).ToString("N2", new CultureInfo("en-IN"));
    }
    public static string RoundWithoutDecimalNepaliFormat(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, MidpointRounding.AwayFromZero).ToString("N0", new CultureInfo("en-IN"));
    }

    public static string RoundToIgnoreTrailingZeroes(this decimal decimalNumber)
    {
        return ((double)decimalNumber).ToString();
    }
    public static string RoundToNepaliIgnoreTrailingZeroes(this decimal decimalNumber)
    {
        return ((double)decimalNumber).ToString("N", new CultureInfo("en-IN"));
    }
    public static string RoundWithoutDecimalEnglishFormat(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, MidpointRounding.AwayFromZero).ToString("N0", new CultureInfo("en-US"));
    }
    public static decimal RoundWithoutDecimal(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber, MidpointRounding.AwayFromZero);
    }
    public static string RoundToTwoEnglishFormat(this decimal decimalNumber) //10.02
    {
        return Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero).ToString("N2", new CultureInfo("en-US"));
    }
    public static string AmountInLakhs(this decimal decimalNumber)
    {
        return Math.Round(decimalNumber /= 100000, 2, MidpointRounding.AwayFromZero).ToString("N2", new CultureInfo("en-IN"));
    }
    public static string AmountInCrore(this decimal decimalNumber)
    {
        var value = Math.Round(decimalNumber /= 10000000, 2, MidpointRounding.AwayFromZero).ToString("N2", new CultureInfo("en-IN"));
        return value;
    }


    // for 2 decimal in print 
    public static string GetFormattedData(this decimal number, int decimalPoints = 4, Culture culture = Culture.Nepali, bool round = true)
    {
        var cultureInfo = GetCulture(culture);

        if (number == decimal.Zero)
        {
            return string.Empty;
        }

        if (decimalPoints == 2)
        {
            if (!round)
            {
                number = Math.Floor(number * 100) / 100;
            }

            return String.Format(cultureInfo, "{0:n2}", number);
        }

        return String.Format(cultureInfo, "{0:n4}", number);
    }

    private static CultureInfo GetCulture(Culture culture)
    {
        switch (culture)
        {
            case Culture.Nepali:
                return _nepaliCulture;
            case Culture.English:
                return _englishCulture;
            default:
                return _nepaliCulture;
        }
    }
}
