using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly EndpointRateLimitBucket MainBucket;
    
    /// <summary>
    /// If not null, use this bucket instead of Bucket if client is LBP PSP.
    /// Need this secondary bucket because PSP uses the same endpoints as LBP1.
    /// </summary>
    public readonly EndpointRateLimitBucket? PspBucket;
    
    public EndpointRateLimitAttribute(EndpointRateLimitBucket bucket, EndpointRateLimitBucket pspBucket)
    {
        this.MainBucket = bucket;
        this.PspBucket = pspBucket;
    }
    
    public EndpointRateLimitAttribute(EndpointRateLimitBucket bucket)
    {
        this.MainBucket = bucket;
    }
}