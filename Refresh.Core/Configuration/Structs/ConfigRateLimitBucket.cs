namespace Refresh.Core.Configuration.Structs;

public struct ConfigRateLimitBucket
{
    public ConfigRateLimitBucket(int timeWindowSeconds, int maxRequestCount, int blockDurationSeconds, string[] endpointRoutes)
    {
        this.TimeWindowSeconds = timeWindowSeconds;
        this.MaxRequestCount = maxRequestCount;
        this.BlockDurationSeconds = blockDurationSeconds;
        this.EndpointRoutes = endpointRoutes;
    }
    
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public string[] EndpointRoutes { get; set; }
}