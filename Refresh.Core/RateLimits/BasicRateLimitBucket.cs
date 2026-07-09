using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.RateLimits;

public class BasicRateLimitBucket
{
    public int TimeWindowSeconds { get; set; }
    public int MaxRequestCount { get; set; }
    public int BlockDurationSeconds { get; set; }
    public string BucketName { get; set; } = "";

    public static BasicRateLimitBucket FromOld(ConfigRateLimitBucket old, string bucketName)
    {
        return new()
        {
            TimeWindowSeconds = old.TimeWindowSeconds,
            MaxRequestCount = old.MaxRequestCount,
            BlockDurationSeconds = old.BlockDurationSeconds,
            BucketName = bucketName,
        };
    }
}