using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;

namespace Business.Common.Helper;
public static class DateHelper
{
    public static string ConvertToLocalDateTime(this string dateTime, string format)
    {
        return Convert.ToDateTime(dateTime).AddHours(5).AddMinutes(45).ToString(format);
    }

    public static DateTime ConvertToLocalDateTime(this string dateTime)
    {
        return Convert.ToDateTime(dateTime).AddHours(5).AddMinutes(45);
    }

    public static string ConvertToLocalDateTime(this DateTime dateTime, string format)
    {
        return dateTime == DateTime.MinValue ? string.Empty : Convert.ToDateTime(dateTime).AddHours(5).AddMinutes(45).ToString(format);
    }
    public static DateTime ConvertToLocalDateTime(this DateTime dateTime, int? endorsementType)
    {
        if (endorsementType == (int)EndorsementListEnum.DateChange)
        {
            return dateTime;
        }
        return dateTime.AddHours(5).AddMinutes(45);
    }
    public static string ConvertToLocalDateTime(this DateTime dateTime, string format, int? endorsementType)
    {
        if (endorsementType == (int)EndorsementListEnum.DateChange)
        {
            return dateTime == DateTime.MinValue ? string.Empty : Convert.ToDateTime(dateTime).ToString(format);
        }
        return dateTime == DateTime.MinValue ? string.Empty : Convert.ToDateTime(dateTime).AddHours(5).AddMinutes(45).ToString(format);
    }
    public static string ConvertToLocalDateTime(this DateTime? dateTime, string format)
    {
        return dateTime.HasValue ? Convert.ToDateTime(dateTime.Value).AddHours(5).AddMinutes(45).ToString(format) : string.Empty;
    }

    public static string FormatDateTime(this DateTime? dateTime, string format)
    {
        return dateTime.HasValue ? Convert.ToDateTime(dateTime.Value).ToString(format) : string.Empty;
    }

    public static string DateOnly(this DateTime? dateTime, string format)
    {
        return dateTime.HasValue ? dateTime.Value.Date.ToString() : string.Empty;
    }

    public static string FormatDateTime(this string dateTime, string format)
    {
        return string.IsNullOrEmpty(dateTime) ? string.Empty : Convert.ToDateTime(dateTime).ToString(format);
    }

    public static string ConvertToNepaliDate(this DateTime? date)
    {
        if (!date.HasValue)
            return "-";
        else if (date.Value == DateTime.MinValue)
            return "-";

        return GetNepaliDate(date.Value);
    }

    public static string ConvertToNepaliDateNonFormatted(this DateTime? date)
    {
        if (!date.HasValue)
            return string.Empty;
        else if (date.Value == DateTime.MinValue)
            return string.Empty;

        return GetNepaliDate(date.Value);
    }

    public static string ConvertToNepaliDateNonFormatted(this string date)
    {
        if (string.IsNullOrEmpty(date))
            return string.Empty;

        DateTime convDate = DateTime.TryParse(date, out DateTime conDate) ? conDate : DateTime.MinValue;
        if (convDate == DateTime.MinValue)
            return string.Empty;

        return GetNepaliDate(convDate);
    }

    public static string ConvertToNepaliDate(this DateTime date)
    {
        if (date == DateTime.MinValue)
            return "-";

        return GetNepaliDate(date);
    }
    public static string ConvertToFromattedNepaliDate(this DateTime date)
    {
        if (date == DateTime.MinValue)
            return "-";

        return GetFormattedNepaliDate(date);
    }

    public static DateTime? ConvertToEnglishDate(this string date)
    {
        if (string.IsNullOrEmpty(date))
            return null;

        string[] dates = date.Split("-");
        if (dates.Length < 3)
            return null;

        int year = int.TryParse(dates[0], out int convYear) ? convYear : 0;
        int month = int.TryParse(dates[1], out int convMonth) ? convMonth : 0;
        int day = int.TryParse(dates[2], out int convDay) ? convDay : 0;
        if (year == 0 & month == 0 & day == 0)
            return null;

        var converted = DateConverter.ConvertToEnglish(year, month, day);
        int engYear = converted.Year;
        int engConvMonth = converted.Month;
        string engMonth = string.Format("{0:D2}", engConvMonth);
        int engConvDay = converted.Day;
        string engDay = string.Format("{0:D2}", engConvDay);
        return DateTime.TryParse($"{engYear}-{engMonth}-{engDay}", out DateTime convEngDate) ? convEngDate : (DateTime?)null;
    }

    private static string GetNepaliDate(DateTime date)
    {
        var converted = DateConverter.ConvertToNepali(date.Year, date.Month, date.Day);
        //if (converted.Year == 0) { return date.ToString("yyy-MM-dd"); }
        int year = converted.Year;
        int convMonth = converted.Month;
        string month = string.Format("{0:D2}", convMonth);
        int convDay = converted.Day;
        string day = string.Format("{0:D2}", convDay);
        return $"{year}-{month}-{day}";
    }
    private static string GetFormattedNepaliDate(DateTime date)
    {
        var converted = DateConverter.ConvertToNepali(date.Year, date.Month, date.Day);
        int year = converted.Year;
        int convMonth = converted.Month;
        string month = string.Format("{0:D2}", convMonth);
        int convDay = converted.Day;
        string day = string.Format("{0:D2}", convDay);
        return $"{day}/{month}/{year}";
    }
    public static DateTime ConvertToUTCDateTime(this string dateTime)
    {
        return Convert.ToDateTime(dateTime).AddHours(-5).AddMinutes(-45);
    }

    public static DateTime ConvertToLocalDateTime(this DateTime dateTime)
    {
        return dateTime.AddHours(5).AddMinutes(45);
    }

    public static DateTime ConvertNepaliToUTCDate(this DateTime dateTime)
    {
        return dateTime.AddHours(-5).AddMinutes(-45);
    }

    public static string ConvertNepaliDateFormat(this DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy");
    }

    public static string ConvertNepaliDateFormat(this string dateTime)
    {
        return Convert.ToDateTime(dateTime).ToString("dd/MM/yyyy");
    }

    public static string ConvertNepaliDateFormatForMultipleDates(this string dateTime)
    {
        if (!string.IsNullOrEmpty(dateTime))
        {
            var individualDates = dateTime.Split(",");
            var resultDateArray = new List<string>();
            foreach (var date in individualDates)
            {
                if (!string.IsNullOrEmpty(date))
                {
                    var formatedDate = Convert.ToDateTime(date).ToString("dd/MM/yyyy");
                    resultDateArray.Add(formatedDate);
                }
            }
            return string.Join(',', resultDateArray);
        }
        return dateTime;
    }

    public static string ConvertNepaliFullDateFormat(this DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy  HH:mm:ss");
    }
    public static string ConvertNepaliTimeOnlyFormat(this DateTime dateTime)
    {
        return dateTime.ToString(" HH:mm:ss");
    }

    public static string ConvertNepaliFULLDateFormat(this string dateTime)
    {
        return Convert.ToDateTime(dateTime).ToString("dd/MM/yyyy  hh:mm:ss");
    }

    public static decimal GetYearDifference(DateTime date1, DateTime date2)
    {
        return Math.Abs(Convert.ToDecimal((date1 - date2).TotalDays) / 365m);
    }

    public static DateViewModel GetPeriodDifference(DateTime date1, DateTime date2)
    {
        DateViewModel dateViewModel = new DateViewModel();
        if (date1.ToShortDateString() == date2.ToShortDateString())
        {
            return dateViewModel;
        }
        DateTime dt3 = new DateTime((date1 - date2).Ticks);
        dateViewModel.PeriodDifference = Math.Abs(Convert.ToDecimal((date1 - date2).TotalDays) / 365m);
        dateViewModel.Years = dt3.Year - 1;
        dateViewModel.Months = dt3.Month - 1;
        dateViewModel.Days = dt3.Day - 1;
        return dateViewModel;
    }

    public static decimal GetYear(DateTime dt1, DateTime dt2)
    {
        if (DateTime.Compare(dt1, dt2) < 0)
        {
            return 0;
        }
        var dt3 = dt1 - dt2;
        DateTime resultDate = DateTime.MinValue + dt3;
        var months = (resultDate.Month - 1) / 12m;
        var result = (resultDate.Year - 1) + months;
        return result;
    }

    public static string GetYearsMonthDayString(DateViewModel model)
    {
        if (model.PeriodDifference == 0)
        {
            return $"{0} year";
        }

        if (model.PeriodDifference < 1)
        {
            string dayText = "day";
            string monthText = "month";

            if (model.Days > 1) { dayText = "days"; }
            if (model.Months > 1) { monthText = "months"; }

            if (model.Months == 0)
            {
                return $"{model.Days} {dayText}";
            }
            if (model.Days == 0)
            {
                return $"{model.Months} {monthText}";
            }
            return $"{model.Months} {monthText} and {model.Days} {dayText} ";
        }

        if (model.PeriodDifference >= 1)
        {
            string yearText = "year";
            if (model.Years > 1) { yearText = "years"; }

            if (model.Months == 0)
            {
                if (model.Days == 0)
                {
                    return $"{model.Years} {yearText}";
                }
                return $"{model.Years} {yearText} and {model.Days} days";
            }
            else
            {
                string monthText = "month";
                string dayText = "day";

                if (model.Months > 1) { monthText = "months"; }
                if (model.Days > 1) { dayText = "days"; }

                if (model.Days == 0)
                {
                    return $"{model.Years} {yearText} and {model.Months} {monthText}";
                }
                return $"{model.Years} {yearText}, {model.Months} {monthText} and {model.Days} {dayText}";
            }
        }
        return $"{0} year"; ;
    }

    public static string GetNepaliYearsMonthDayString(DateViewModel model)
    {
        string nepaliYear = model.Years.ToString();
            //.ConvertNumerals("ar-SA");
        string nepaliMonth = model.Months.ToString();
            //.ConvertNumerals("ar-SA");
        string nepaliDay = model.Days.ToString();
            //.ConvertNumerals("ar-SA");

        string yearText = "वर्ष";
        string monthText = "महिना";
        string dayText = "दिन";

        if (model.PeriodDifference == 0)
        {
            return $"{0} {yearText}";
        }

        if (model.PeriodDifference < 1)
        {
            if (model.Months == 0)
            {
                return $"{nepaliDay} {dayText}";
            }
            if (model.Days == 0)
            {
                return $"{nepaliMonth} {monthText}";
            }
            return $"{nepaliMonth} {monthText} र {nepaliDay} {dayText} ";
        }

        if (model.PeriodDifference >= 1)
        {
            if (model.Months == 0)
            {
                if (model.Days == 0)
                {
                    return $"{nepaliYear} {yearText}";
                }
                return $"{nepaliYear} {yearText}";
            }
            else
            {
                if (model.Days == 0)
                {
                    return $"{nepaliYear} {yearText}";
                }
                return $"{nepaliYear} {yearText}";
            }
            //return $"{nepaliYear} {yearText}";
        }
        return $"{0} {yearText}"; ;
    }

    public static string GetEnglishYearsMonthDayString(DateViewModel model)
    {
        string englishYear = (model.Years.ToString());
        string englishMonth = (model.Months.ToString());
        string englishDay = (model.Days.ToString());
        string yearText = "Year";

        string monthText = "Month";
        string dayText = "Day";

        if (model.PeriodDifference == 0)
        {
            return $"{0} {yearText}";
        }

        if (model.PeriodDifference < 1)
        {
            if (model.Months == 0)
            {
                return $"{englishDay} {dayText}";
            }
            if (model.Days == 0)
            {
                return $"{englishMonth} {monthText}";
            }
            return $"{englishMonth} {monthText} & {englishDay} {dayText} ";
        }

        if (model.PeriodDifference >= 1)
        {
            if (model.Months == 0)
            {
                if (model.Days == 0)
                {
                    return $"{englishYear} {yearText}";
                }
                return $"{englishYear} {yearText}";
            }
            else
            {
                if (model.Days == 0)
                {
                    return $"{englishYear} {yearText}";
                }
                return $"{englishYear} {yearText}";
            }
            // return $"{englishYear} {yearText}";
        }
        return $"{0} {yearText}"; ;
    }

    public static string GetAge(DateViewModel date)
    {
        if (date.Years > 0) return string.Format("{0} Years", date.Years);
        if (date.Months > 0) return string.Format("{0} Months", date.Months);
        return date.Days + " Days ";
    }

    public static string GetFormattedDate(DateTime? effectiveDate)
    {
        var formattedEffectiveDate = effectiveDate != null ? Convert.ToDateTime(effectiveDate).ConvertNepaliDateFormat() : null;
        return formattedEffectiveDate;
    }
    public static string GetFormattedTimeOnly(DateTime? effectiveDate)
    {
        var formattedEffectiveDate = effectiveDate != null ? Convert.ToDateTime(effectiveDate).ConvertNepaliTimeOnlyFormat() : null;
        return formattedEffectiveDate;
    }

    public static string GetProposedDate(DateTime? date, DateTime? formattedEffectiveDate)
    {
        var proposedDate = date.Value.Date != new DateTime().Date ? Convert.ToDateTime(date).ConvertNepaliDateFormat() : GetFormattedDate(formattedEffectiveDate);
        return proposedDate;
    }

    public static string GetFormattedDateInNepali(this DateTime? effectiveDate)
    {
        return GetFormattedDate(effectiveDate);
            //.ConvertNumerals("ar-SA");
    }

    public static string GetProposedDateInNepali(DateTime? date, DateTime? formattedEffectiveDate)
    {
        return GetProposedDate(date, formattedEffectiveDate);//.ConvertNumerals("ar-SA");
    }
    public static DateTime GetEffectiveDateForRenewal(DateTime previousExpiryDate)
    {
        return previousExpiryDate.Date < DateTime.Now.Date ? DateTime.Now : previousExpiryDate.AddDays(1);
    }
}
