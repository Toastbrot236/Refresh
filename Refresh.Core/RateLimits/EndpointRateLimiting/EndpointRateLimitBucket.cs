using Refresh.Core.Configuration;
using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public class EndpointRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public EndpointBucketName BucketName { get; set; }

    public EndpointRateLimitBucket(ConfigRateLimitBucket old, EndpointBucketName bucketName)
    {
        this.TimeWindowSeconds = old.TimeWindowSeconds;
        this.MaxRequestCount = old.MaxRequestCount;
        this.BlockDurationSeconds = old.BlockDurationSeconds;
        this.BucketName = bucketName;
    }
}