using System.Net;

namespace Refresh.Core.RateLimits.Info;

public class RateLimitRemoteEndpointInfo : IRateLimitInfo
{
    public RateLimitRemoteEndpointInfo(IPAddress ipAddress, BucketName bucket)
    {
        this.IpAddress = ipAddress;
        this.Bucket = bucket;
    }

    internal IPAddress IpAddress { get; init; }
    public List<int> RequestTimes { get; init; } = new(25);
    public int LimitedUntil { get; set; }
    public BucketName Bucket { get; init; }
}