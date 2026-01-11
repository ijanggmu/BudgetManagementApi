using System.Globalization;
using Models.Common.Policy.Enum;

namespace SharedKernel.Helper;
public static class DecimalHelper
{
    private static readonly CultureInfo _englishCulture = CultureInfo.InvariantCulture;

    #region Rounding Only

    public static decimal RoundToFour(this decimal decimalNumber)
        => Math.Round(decimalNumber, 4, MidpointRounding.AwayFromZero);

    public static decimal RoundToFourPrecisions(this decimal decimalNumber)
        => Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero);

    public static decimal RoundToThree(this decimal decimalNumber)
        => Math.Round(decimalNumber, 3, MidpointRounding.AwayFromZero);

    public static decimal RoundToSix(this decimal decimalNumber)
        => Math.Round(decimalNumber, 6, MidpointRounding.AwayFromZero);

    public static decimal RoundToEight(this decimal decimalNumber)
        => Math.Round(decimalNumber, 8, MidpointRounding.AwayFromZero);

    public static decimal RoundToTwo(this decimal decimalNumber)
        => Math.Round(decimalNumber, 2, MidpointRounding.AwayFromZero);

    public static decimal RoundWithoutDecimal(this decimal decimalNumber)
        => Math.Round(decimalNumber, MidpointRounding.AwayFromZero);

    #endregion

    #region Nepali / Indian Formatting (Invariant Safe)

    public static string RoundToFourNepaliFormat(this decimal decimalNumber)
        => FormatIndian(decimalNumber, 4);

    public static string RoundToThreeNepaliFormat(this decimal decimalNumber)
        => FormatIndian(decimalNumber, 3);

    public static string RoundToTwoNepaliFormat(this decimal decimalNumber)
        => FormatIndian(decimalNumber, 2);

    public static string RoundToTwoDecimalFormat(this decimal decimalNumber)
        => FormatIndian(decimalNumber, 2);

    public static string RoundWithoutDecimalNepaliFormat(this decimal decimalNumber)
        => FormatIndian(decimalNumber, 0);

    public static string RoundToNepaliIgnoreTrailingZeroes(this decimal decimalNumber)
        => TrimTrailingZeros(FormatIndian(decimalNumber, 4));

    public static string AmountInLakhs(this decimal decimalNumber)
        => FormatIndian(decimalNumber / 100000m, 2);

    public static string AmountInCrore(this decimal decimalNumber)
        => FormatIndian(decimalNumber / 10000000m, 2);

    #endregion

    #region English Formatting

    public static string RoundWithoutDecimalEnglishFormat(this decimal decimalNumber)
        => decimalNumber.RoundWithoutDecimal().ToString("N0", _englishCulture);

    public static string RoundToTwoEnglishFormat(this decimal decimalNumber)
        => decimalNumber.RoundToTwo().ToString("N2", _englishCulture);

    #endregion

    #region Misc

    public static string RoundToIgnoreTrailingZeroes(this decimal decimalNumber)
        => decimalNumber.ToString(_englishCulture);

    // kept signature unchanged
    public static string GetFormattedData(this decimal number, int decimalPoints = 4, Culture culture = Culture.Nepali, bool round = true)
    {
        if (number == decimal.Zero)
            return string.Empty;

        if (!round && decimalPoints == 2)
            number = Math.Floor(number * 100) / 100;

        return culture == Culture.English
            ? number.ToString($"N{decimalPoints}", _englishCulture)
            : FormatIndian(number, decimalPoints);
    }

    #endregion

    #region Core Formatter (NO Culture Dependency)

    private static string FormatIndian(decimal value, int decimals)
    {
        var isNegative = value < 0;
        value = Math.Abs(value);

        value = decimals > 0
            ? Math.Round(value, decimals, MidpointRounding.AwayFromZero)
            : Math.Round(value, MidpointRounding.AwayFromZero);

        var parts = value
            .ToString($"F{decimals}", CultureInfo.InvariantCulture)
            .Split('.');

        var integerPart = parts[0];
        var decimalPart = parts.Length > 1 ? "." + parts[1] : string.Empty;

        if (integerPart.Length <= 3)
            return (isNegative ? "-" : "") + integerPart + decimalPart;

        var lastThree = integerPart[^3..];
        var remaining = integerPart[..^3];

        var groups = new Stack<string>();
        while (remaining.Length > 2)
        {
            groups.Push(remaining[^2..]);
            remaining = remaining[..^2];
        }

        if (remaining.Length > 0)
            groups.Push(remaining);

        var formatted = string.Join(",", groups) + "," + lastThree;

        return (isNegative ? "-" : "") + formatted + decimalPart;
    }

    private static string TrimTrailingZeros(string value)
    {
        if (!value.Contains('.'))
            return value;

        value = value.TrimEnd('0');
        return value.EndsWith('.') ? value[..^1] : value;
    }

    #endregion
}

