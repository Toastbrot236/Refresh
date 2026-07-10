using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.RateLimits;

public class BasicRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public BucketName Name { get; set; }

    public BasicRateLimitBucket(ConfigRateLimitBucket old, BucketName bucketName)
    {
        this.TimeWindowSeconds = old.TimeWindowSeconds;
        this.MaxRequestCount = old.MaxRequestCount;
        this.BlockDurationSeconds = old.BlockDurationSeconds;
        this.Name = bucketName;
    }
}