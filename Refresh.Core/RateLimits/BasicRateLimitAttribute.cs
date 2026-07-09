namespace Refresh.Core.RateLimits;

[AttributeUsage(AttributeTargets.Method)]
public class BasicRateLimitAttribute : Attribute
{
    public readonly string Bucket;
    
    public BasicRateLimitAttribute(string bucket)
    {
        this.Bucket = bucket;
    }
}