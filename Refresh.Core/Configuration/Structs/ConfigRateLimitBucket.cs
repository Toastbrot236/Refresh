namespace Refresh.Core.Configuration.Structs;

public struct ConfigRateLimitBucket
{
    public ConfigRateLimitBucket(int timeWindowSeconds, int maxRequestCount, int blockDurationSeconds)
    {
        TimeWindowSeconds = timeWindowSeconds;
        MaxRequestCount = maxRequestCount;
        BlockDurationSeconds = blockDurationSeconds;
    }
    
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
}