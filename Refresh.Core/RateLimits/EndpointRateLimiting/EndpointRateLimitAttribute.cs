namespace Refresh.Core.RateLimits.EndpointRateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointRateLimitAttribute : Attribute
{
    public readonly BucketName Bucket;
    
    public EndpointRateLimitAttribute(BucketName bucket)
    {
        this.Bucket = bucket;
    }
}