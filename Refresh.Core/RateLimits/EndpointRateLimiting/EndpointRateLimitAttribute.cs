namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly GameEndpointBucketName? GameBucket;
    public readonly ApiEndpointBucketName? ApiBucket;
    
    public EndpointRateLimitAttribute(GameEndpointBucketName bucket)
    {
        this.GameBucket = bucket;
    }
    
    public EndpointRateLimitAttribute(ApiEndpointBucketName bucket)
    {
        this.ApiBucket = bucket;
    }
}