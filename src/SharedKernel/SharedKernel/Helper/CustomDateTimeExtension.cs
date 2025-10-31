namespace SharedKernel.Helper;

public static class CustomDateTimeExtension
{
    public static string ConvertUtcToNepalDateTimeFormat(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.AddHours(5).AddMinutes(45).ToString("yyyy-MM-dd hh:mm tt");
    }

    public static string ConvertUtcToNepalDateTimeFormat(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset == null ? string.Empty : dateTimeOffset?.AddHours(5).AddMinutes(45).ToString("yyyy-MM-dd hh:mm tt");
    }

    public static string ConvertUtcToNepalDate(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.AddHours(5).AddMinutes(45).ToString("yyyy-MM-dd");
    }

    public static string ConvertUtcToNepalDate(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset == null ? string.Empty : dateTimeOffset?.AddHours(5).AddMinutes(45).ToString("yyyy-MM-dd");
    }

    public static string ToUtcString(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss zzz");
    }

    public static string ToUtcString(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset == null ? null : dateTimeOffset?.ToString("yyyy-MM-dd HH:mm:ss zzz");
    }

    public static string To12HourUtcString(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("yyyy-MM-dd hh:mm:ss tt");
    }

    public static string To12HourUtcDateString(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("yyyy-MM-dd");
    }

    public static string To12HourUtcString(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset == null ? null : dateTimeOffset?.ToString("yyyy-MM-dd hh:mm:ss tt");
    }

    //public static string ConvertToNepaliDate(DateTime date)
    //{
    //    if (date != DateTime.MinValue)
    //    {
    //        var localDateTime = date.ToLocalTime();
    //        var nepaliDate = NepaliDateConverter.DateConverter.ConvertToNepali(localDateTime.Year, localDateTime.Month, localDateTime.Day);
    //        var nepaliDay = nepaliDate.Day < 10 ? $"0{nepaliDate.Day}" : nepaliDate.Day.ToString();
    //        var nepaliMonth = nepaliDate.Month < 10 ? $"0{nepaliDate.Month}" : nepaliDate.Month.ToString();

    //        return $"{nepaliDate.Year}.{nepaliMonth}.{nepaliDay}";

    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}

}

