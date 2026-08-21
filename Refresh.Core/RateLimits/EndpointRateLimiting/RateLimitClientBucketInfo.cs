using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public class RateLimitClientBucketInfo<TClientType> : IRateLimitClientBucketInfo
{
    public List<int> RequestTimes { get; init; } = new(25);
    public int LimitedUntil { get; set; }
    public TClientType ClientId { get; init; }
    public EndpointBucketName Bucket { get; init; }
    
    public RateLimitClientBucketInfo(TClientType clientId, EndpointBucketName bucket)
    {
        this.ClientId = clientId;
        this.Bucket = bucket;
    }
}