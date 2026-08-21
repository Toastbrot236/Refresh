using System.Net;
using System.Reflection;
using System.Collections.Frozen;
using Bunkum.Listener.Request;
using MongoDB.Bson;
using NotEnoughLogs;
using Refresh.Common;
using Refresh.Common.Time;
using Refresh.Core.Configuration;
using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;
using Refresh.Database.Models.Users;

namespace Refresh.Core.RateLimits.EndpointRateLimiting ;

public class EndpointRateLimiter
{
    private readonly Logger _logger;
    private readonly IDateTimeProvider _timeProvider;
    private readonly FrozenDictionary<EndpointBucketName, ConfigRateLimitBucket> _buckets;
    
    private readonly List<RateLimitClientBucketInfo<ObjectId>> _userInfos = new(25);
    private readonly List<RateLimitClientBucketInfo<IPAddress>> _remoteEndpointInfos = new(25);
    
    public EndpointRateLimiter(IDateTimeProvider timeProvider, Logger logger, Dictionary<string, ConfigRateLimitBucket> configBuckets)
    {
        this._timeProvider = timeProvider;
        this._logger = logger;

        // Copy the buckets over, converting the string bucket names to their corresponding enum values.
        Dictionary<EndpointBucketName, ConfigRateLimitBucket> validBuckets = new();
        
        foreach (KeyValuePair<string, ConfigRateLimitBucket> bucket in configBuckets)
        {
            bool parsed = Enum.TryParse(bucket.Key, true, out EndpointBucketName nameParsed);
            if (!parsed)
            {
                this._logger.LogDebug(RefreshContext.RateLimit, $"Bucket name '{bucket.Key}' found in rate-limit config is unknown (does not map to a valid {nameof(EndpointBucketName)} enum value), its bucket will be ignored.");
                continue;
            }
            
            validBuckets.Add(nameParsed, bucket.Value);
        }
        
        this._buckets = validBuckets.ToFrozenDictionary();
    }

    private EndpointRateLimitBucket GetBucketNameAndData(ListenerContext context, MethodInfo? method, bool isPsp)
    {
        EndpointRateLimitAttribute? attribute = method?.GetCustomAttribute<EndpointRateLimitAttribute>();
        
        EndpointBucketName bucketName = EndpointBucketName.Default;
        if (attribute != null) bucketName = isPsp ? attribute.PspBucket : attribute.MainBucket;
        
        ConfigRateLimitBucket? bucketData = this._buckets.GetValueOrDefault(bucketName);

        if (bucketData == null)
        {
            this._logger.LogDebug(RefreshContext.RateLimit, $"Could not find bucket '{bucketName}' in config, falling back to hardcoded defaults.");
            bucketData = EndpointBucketDefaults.Buckets.GetValueOrDefault(bucketName);
            
            if (bucketData == null)
            {
                throw new NotImplementedException($"Could not find bucket '{bucketName}' in neither the config file nor the hardcoded defaults! You should open an issue about this.");
            }
        }
        
        return new EndpointRateLimitBucket(bucketName, bucketData);
    }

    public bool UserViolatesRateLimit(ListenerContext context, MethodInfo method, bool isPsp, GameUser user)
    {
        EndpointRateLimitBucket bucket = this.GetBucketNameAndData(context, method, isPsp);

        lock (this._userInfos)
        {
            RateLimitClientBucketInfo<ObjectId>? info = this._userInfos.FirstOrDefault(i =>
                    user.UserId.Equals(i.ClientId) && i.Bucket == bucket.Name);

            if (info == null)
            {
                info = new RateLimitClientBucketInfo<ObjectId>(user.UserId, bucket.Name);
                this._userInfos.Add(info);
            }

            lock (info)
            {
                return this.ViolatesRateLimit(context, bucket, info, user);
            }
        }
    }

    public bool RemoteEndpointViolatesRateLimit(ListenerContext context, MethodInfo method)
    {
        IPAddress ipAddress = context.RemoteEndpoint.Address;
        
        EndpointRateLimitBucket bucket = this.GetBucketNameAndData(context, method, false);

        lock (this._remoteEndpointInfos)
        {
            RateLimitClientBucketInfo<IPAddress>? info = this._remoteEndpointInfos
                .FirstOrDefault(i => ipAddress.Equals(i.ClientId) && i.Bucket == bucket.Name);

            if (info == null)
            {
                info = new RateLimitClientBucketInfo<IPAddress>(ipAddress, bucket.Name);
                this._remoteEndpointInfos.Add(info);
            }

            lock (info)
            {
                return this.ViolatesRateLimit(context, bucket, info, null);
            }
        }
    }

    public bool ViolatesRateLimit(ListenerContext context, EndpointRateLimitBucket bucket, IRateLimitClientBucketInfo info, GameUser? user)
    {
        int now = (int)this._timeProvider.TimestampSeconds;
        
        // Always clear all expired and add this request as a new one,
        // to make the block duration longer the more the client continues to spam
        info.RequestTimes.RemoveAll(r => r <= now - bucket.Data.TimeWindowSeconds);
        info.RequestTimes.Add(now);
        
        // Repeat block
        // If a rate-limit is already triggered, block regardless of whether MaxRequestCount has been reached
        if (info.LimitedUntil != 0 && info.LimitedUntil > now)
            return true;
        
        // Initial block
        if (info.RequestTimes.Count > bucket.Data.MaxRequestCount)
        {
            info.LimitedUntil = now + bucket.Data.BlockDurationSeconds;
            context.ResponseHeaders.TryAdd("Retry-After", bucket.Data.BlockDurationSeconds.ToString()); // TODO also include overshot time
            
            return true;
        }
        
        return false;
    }
}