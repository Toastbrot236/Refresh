namespace Refresh.Core.RateLimits;

[AttributeUsage(AttributeTargets.Method)]
public class BasicRateLimitAttribute : Attribute
{
    public readonly BucketName Bucket;
    
    public BasicRateLimitAttribute(BucketName bucket)
    {
        this.Bucket = bucket;
    }
}