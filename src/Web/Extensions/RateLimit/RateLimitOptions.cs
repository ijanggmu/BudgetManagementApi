namespace BeemaEdgeApi.Extensions.RateLimit;

public class RateLimitOptions
{
    public bool EnableRateLimiting { get; init; }
    public int PermitLimitInMinutes { get; init; }
    public int PermitLimitInHours { get; init; }
    public int WindowInMinutes { get; init; }
    public int WindowInHours { get; init; }
    public int RejectionStatusCode { get; init; }
}
