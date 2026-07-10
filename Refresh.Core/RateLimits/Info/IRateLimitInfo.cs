namespace Refresh.Core.RateLimits.Info;

// For some reason, this interface is internal in Bunkum.Core, so we have to copy it.
public interface IRateLimitInfo
{
    internal List<int> RequestTimes { get; init; }
    internal int LimitedUntil { get; set; }
    public BucketName Bucket { get; init; }
}