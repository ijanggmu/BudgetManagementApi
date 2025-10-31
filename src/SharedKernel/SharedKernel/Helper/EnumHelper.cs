using System.ComponentModel.DataAnnotations;
using SharedKernel.Models.DropDown;
namespace SharedKernel.Helper;
public static partial class EnumHelper
{
    public static string GetDisplayName<T>(T value) where T : Enum
    {
        var memberInfo = typeof(T).GetMember(value.ToString()).FirstOrDefault();
        var attributes = memberInfo?.GetCustomAttributes(typeof(DisplayAttribute), false);
        var displayName = ((DisplayAttribute)attributes?.FirstOrDefault())?.Name;
        return displayName ?? value.ToString();
    }

    public static List<TDropDownList> GetDropDownList<TEnum, TDropDownList>() where TEnum : Enum
    where TDropDownList : CommonDropDownListIntResponseModel, new()
    {
        var enumValues = Enum.GetValues(typeof(TEnum)).OfType<TEnum>();
        var dropDownList = enumValues.Select(enumValue => new TDropDownList
        {
            Key = Convert.ToInt32(enumValue),
            Value = Convert.ToInt32(enumValue),
            Label = GetDisplayName(enumValue)
        }).ToList();

        return dropDownList;
    }
    public static bool IsValidEnumValue<T>(this T value) where T : struct, Enum
    {
        return Enum.IsDefined(typeof(T), value);
    }
}
