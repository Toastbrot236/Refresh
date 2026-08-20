using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public class EndpointRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public EndpointRateLimitBucket BucketName { get; set; }

    public EndpointRateLimitBucket(ConfigRateLimitBucket old, EndpointRateLimitBucket bucketName)
    {
        this.TimeWindowSeconds = old.TimeWindowSeconds;
        this.MaxRequestCount = old.MaxRequestCount;
        this.BlockDurationSeconds = old.BlockDurationSeconds;
        this.BucketName = bucketName;
    }
}