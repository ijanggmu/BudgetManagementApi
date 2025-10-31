namespace SharedKernel.Config;

public sealed class CacheOptions
{
    public bool IsEnabled { get; set; }
    public string CacheType { get; set; }
    public int AbsoluteExpirationInHours { get; set; }
    public int SlidingExpirationInSeconds { get; set; }
}
