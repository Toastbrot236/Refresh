using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly EndpointBucketName MainBucket;
    
    /// <summary>
    /// If not null, use this bucket instead of Bucket if client is LBP PSP.
    /// Need this secondary bucket because PSP uses the same endpoints as LBP1.
    /// </summary>
    public readonly EndpointBucketName? PspBucket;
    
    public EndpointRateLimitAttribute(EndpointBucketName bucket, EndpointBucketName pspBucket)
    {
        this.MainBucket = bucket;
        this.PspBucket = pspBucket;
    }
    
    public EndpointRateLimitAttribute(EndpointBucketName bucket)
    {
        this.MainBucket = bucket;
    }
}