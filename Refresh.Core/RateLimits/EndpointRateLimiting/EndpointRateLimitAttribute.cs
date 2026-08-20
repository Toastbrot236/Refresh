using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly GameEndpointBucketName? GameBucket;
    
    /// <summary>
    /// If not null, use this bucket instead of GameBucket if client is LBP PSP
    /// </summary>
    public readonly GameEndpointBucketName? PspBucket;
    
    public readonly ApiEndpointBucketName? ApiBucket;
    
    public EndpointRateLimitAttribute(GameEndpointBucketName bucket, GameEndpointBucketName pspBucket)
    {
        this.GameBucket = bucket;
        this.PspBucket = pspBucket;
    }
    
    public EndpointRateLimitAttribute(GameEndpointBucketName bucket)
    {
        this.GameBucket = bucket;
    }
    
    public EndpointRateLimitAttribute(ApiEndpointBucketName bucket)
    {
        this.ApiBucket = bucket;
    }
}