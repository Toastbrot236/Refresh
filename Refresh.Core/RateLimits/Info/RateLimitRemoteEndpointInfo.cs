using System.Net;

namespace Refresh.Core.RateLimits.Info;

// For some reason, this class, unlike the others, is NOT internal in Bunkum.Core, but we're copying it anyway for consistency.
public class RateLimitRemoteEndpointInfo : IRateLimitInfo
{
    public RateLimitRemoteEndpointInfo(IPAddress ipAddress, string bucket)
    {
        this.IpAddress = ipAddress;
        this.Bucket = bucket;
    }

    internal IPAddress IpAddress { get; init; }
    public List<int> RequestTimes { get; init; } = new(25);
    public int LimitedUntil { get; set; }
    public string Bucket { get; init; }
}