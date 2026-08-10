namespace Refresh.Core.RateLimits.Info;

public interface IRateLimitInfo
{
    internal List<int> RequestTimes { get; init; }
    internal int LimitedUntil { get; set; }
    public BucketName Bucket { get; init; }
}