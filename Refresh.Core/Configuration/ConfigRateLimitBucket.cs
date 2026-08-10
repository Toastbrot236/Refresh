namespace Refresh.Core.Configuration;

public class ConfigRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }

    public ConfigRateLimitBucket(int timeWindowSeconds, int maxRequestCount, int blockDurationSeconds)
    {
        this.TimeWindowSeconds = timeWindowSeconds;
        this.MaxRequestCount = maxRequestCount;
        this.BlockDurationSeconds = blockDurationSeconds;
    }
}