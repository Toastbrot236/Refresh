using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public interface IRateLimitClientBucketInfo
{
    public List<int> RequestTimes { get; init; }
    public int LimitedUntil { get; set; }
    public EndpointBucketName Bucket { get; init; }
}