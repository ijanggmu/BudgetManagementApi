namespace SharedKernel.Models.DropDown;

public abstract class DropDownListResponseModel<T>
{
    public T Key { get; set; } = default;
    public string Label { get; set; }
    public T Value { get; set; } = default;
}

