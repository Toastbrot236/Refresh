using System.Collections.Frozen;
using System.Net;
using System.Reflection;
using Bunkum.Core.RateLimit;
using Bunkum.Listener.Request;
using NotEnoughLogs;
using Refresh.Common;
using Refresh.Common.Time;
using Refresh.Core.Configuration;
using Refresh.Core.Configuration.Structs;
using Refresh.Core.RateLimits.Info;
using Refresh.Database.Models.Authentication;

namespace Refresh.Core.RateLimits;

public class BasicRateLimiter : IRateLimiter
{
    private readonly Logger _logger;
    private readonly IDateTimeProvider _timeProvider;
    private readonly FrozenDictionary<string, ConfigRateLimitBucket> _buckets;
    
    public BasicRateLimiter(IDateTimeProvider timeProvider, Logger logger, Dictionary<string, ConfigRateLimitBucket> buckets)
    {
        this._timeProvider = timeProvider;
        this._logger = logger;
        this._buckets = buckets.ToFrozenDictionary();
    }

    private readonly List<RateLimitUserInfo> _userInfos = new(25);
    private readonly List<RateLimitRemoteEndpointInfo> _remoteEndpointInfos = new(25);

    private BasicRateLimitBucket GetBucket(MethodInfo? method, TokenGame game)
    {
        string bucketName = method?.GetCustomAttribute<BasicRateLimitAttribute>()?.Bucket ?? "global";
        
        // If we're on PSP, then find out if there is a PSP-specific bucket for this, and override with the PSP-specific name.
        // This is because PSP uses the same endpoints as LBP1 (and other games in some cases), so we can just use the regular
        // game bucket name on the endpoints and have it be corrected here.
        if (game == TokenGame.LittleBigPlanetPSP)
        {
            bucketName = BucketDefaults.PspNameOverrides.GetValueOrDefault(bucketName) ?? bucketName;
        }
        
        ConfigRateLimitBucket? bucketData = this._buckets.GetValueOrDefault(bucketName);

        if (bucketData == null)
        {
            this._logger.LogDebug(RefreshContext.RateLimit, $"Could not find bucket '{bucketName}' in config, falling back to hardcoded defaults.");
            bucketData = BucketDefaults.Values.GetValueOrDefault(bucketName);
            
            if (bucketData == null)
            {
                throw new NotImplementedException($"Could not find bucket '{bucketName}' in neither the config file nor the hardcoded defaults!");
            }
        }
        
        return BasicRateLimitBucket.FromOld(bucketData.Value, bucketName);
    }

    public bool UserViolatesRateLimit(ListenerContext context, MethodInfo? method, TokenGame game, IRateLimitUser user)
    {
        BasicRateLimitBucket bucket = this.GetBucket(method, game);

        lock (this._remoteEndpointInfos)
        {
            RateLimitUserInfo? info = this._userInfos
                .FirstOrDefault(i =>
                    user.RateLimitUserIdIsEqual(i.User.RateLimitUserId) && i.Bucket == bucket.BucketName);

            if (info == null)
            {
                info = new RateLimitUserInfo(user, bucket.BucketName);
                lock (this._userInfos)
                {
                    this._userInfos.Add(info);
                }
            }

            lock (info)
            {
                return this.ViolatesRateLimit(context, info, bucket);
            }
        }
    }

    public bool RemoteEndpointViolatesRateLimit(ListenerContext context, MethodInfo? method, TokenGame game)
    {
        IPAddress ipAddress = context.RemoteEndpoint.Address;
        
        BasicRateLimitBucket bucket = this.GetBucket(method);

        lock (this._remoteEndpointInfos)
        {
            RateLimitRemoteEndpointInfo? info = this._remoteEndpointInfos
                .FirstOrDefault(i => ipAddress.Equals(i.IpAddress) && i.Bucket == bucket.BucketName);

            if (info == null)
            {
                info = new RateLimitRemoteEndpointInfo(ipAddress, bucket.BucketName);

                this._remoteEndpointInfos.Add(info);
            }

            lock (info)
            {
                return this.ViolatesRateLimit(context, info, bucket);
            }
        }
    }

    private bool ViolatesRateLimit(ListenerContext context, IRateLimitInfo info, BasicRateLimitBucket bucket)
    {
        int now = (int)this._timeProvider.TimestampSeconds;
        
        // Always clear all expired and add this request as a new one,
        // to make the block duration longer the more the client continues to spam
        info.RequestTimes.RemoveAll(r => r <= now - bucket.TimeWindowSeconds);
        info.RequestTimes.Add(now);
        
        // Repeat block
        // If a rate-limit is already triggered, block regardless of whether MaxRequestCount has been reached
        if (info.LimitedUntil != 0 && info.LimitedUntil > now)
            return true;
        
        // Initial block
        if (info.RequestTimes.Count > bucket.MaxRequestCount)
        {
            info.LimitedUntil = now + bucket.BlockDurationSeconds;
            context.ResponseHeaders.TryAdd("Retry-After", bucket.BlockDurationSeconds.ToString()); // TODO also include overshot time
            
            return true;
        }
        
        return false;
    }
}