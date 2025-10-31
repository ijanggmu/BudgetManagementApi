namespace SharedKernel.Config;

public class HmacAuthSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int AllowedClockSkewMinutes { get; set; } = 5;
}
