namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public class RateLimitClientBucketInfo<TClientType>
{
    internal List<int> RequestTimes { get; init; } = new(25);
    internal int LimitedUntil { get; set; }
    public TClientType ClientId { get; init; }
    public EndpointRateLimitBucket Bucket { get; init; }
    
    public RateLimitClientBucketInfo(TClientType clientId, EndpointRateLimitBucket bucket)
    {
        this.ClientId = clientId;
        this.Bucket = bucket;
    }
}