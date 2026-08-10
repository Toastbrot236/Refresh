using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public class EndpointRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public BucketName Name { get; set; }

    public EndpointRateLimitBucket(ConfigRateLimitBucket old, BucketName bucketName)
    {
        this.TimeWindowSeconds = old.TimeWindowSeconds;
        this.MaxRequestCount = old.MaxRequestCount;
        this.BlockDurationSeconds = old.BlockDurationSeconds;
        this.Name = bucketName;
    }
}