using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly EndpointBucketName MainBucket;
    
    /// <summary>
    /// If the client is LBP PSP, use this bucket instead of MainBucket.
    /// We need this secondary bucket because LBP PSP uses the same endpoints as LBP1,
    /// while also sending unreasonably higher amounts of requests to certain endpoints
    /// in certain cases.
    /// </summary>
    public readonly EndpointBucketName PspBucket;
    
    public EndpointRateLimitAttribute(EndpointBucketName bucket, EndpointBucketName pspBucket)
    {
        this.MainBucket = bucket;
        this.PspBucket = pspBucket;
    }
    
    public EndpointRateLimitAttribute(EndpointBucketName bucket)
    {
        this.MainBucket = bucket;
        this.PspBucket = bucket;
    }
}