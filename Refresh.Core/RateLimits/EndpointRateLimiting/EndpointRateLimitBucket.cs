using Refresh.Core.Configuration;
using Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

namespace Refresh.Core.RateLimits.EndpointRateLimiting;

public record EndpointRateLimitBucket(EndpointBucketName Name, ConfigRateLimitBucket Data);