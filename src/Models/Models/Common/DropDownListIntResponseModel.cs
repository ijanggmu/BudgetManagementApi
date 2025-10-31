namespace Models.Common;

public class CommonDropDownListResponseModel: DropDownListResponseModel<string>;
public class CommonDropDownListIntResponseModel: DropDownListResponseModel<int?>;
public abstract class DropDownListResponseModel<T>
{
    public T Key { get; set; } = default;
    public string Label { get; set; }
    public T Value { get; set; } = default;
}
