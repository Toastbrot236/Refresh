using Bunkum.Core.RateLimit;

namespace Refresh.Core.RateLimits.Info;

// For some reason, this class is internal in Bunkum.Core, so we have to copy it.
public class RateLimitUserInfo : IRateLimitInfo
{
    internal RateLimitUserInfo(IRateLimitUser user, BucketName bucket)
    {
        this.User = user;
        this.Bucket = bucket;
    }

    internal IRateLimitUser User { get; init; }
    public BucketName Bucket { get; init; }
    public List<int> RequestTimes { get; init; } = new(25);
    public int LimitedUntil { get; set; }
}