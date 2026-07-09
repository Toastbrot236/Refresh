using System.Collections.Frozen;
using Bunkum.Core.Configuration;
using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.Configuration;

public class BasicEndpointRateLimitConfig : Config
{
    public override int CurrentConfigVersion => 1;
    public override int Version { get; set; }
    
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {
        
    }

    // bucket name -> rest of bucket data
    // They're stored like this here to try and make it easier for an instance owner to read,
    // and to make the bucket name's purpose more obvious.
    public Dictionary<string, ConfigRateLimitBucket> Buckets { get; set; } = new();
    
    // TODO once we can write into configs outside of migrations, use this to auto-add buckets incase they're missing for some reason
    // (e.g. instance owner deleted them, but why would you even do that?)
    /// <summary>
    /// Takes the given dictionary, and inserts all entries (buckets, by using their keys) that are missing
    /// </summary>
    public static Dictionary<string, ConfigRateLimitBucket> InsertDefaults(Dictionary<string, ConfigRateLimitBucket> input)
    {
        // TODO
        return input;
    }

    public static readonly FrozenDictionary<string, ConfigRateLimitBucket> DefaultBuckets = new Dictionary<string, ConfigRateLimitBucket>()
    {
        // TODO: consider whether deletion endpoints should even be rate-limited if all they do is delete an entity.
        // TODO: also consider whether moderation endpoints should be rate-limited. I don't really think so, but maybe there could be a good reason for it?
        // NOTE: Bucket names are prefixed by where the bucket is used (game/certain game/API/all)
        // Miscellaneous
        {"global", new(90, 380, 45)},
        
        // Levels
        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {"gameStartPublishLevel", new(900, 15, 600)},
        {"gameFullyPublishLevel", new(900, 15, 600)},
        {"gameDeleteLevel", new(600, 15, 300)},
        // TODO finish the list
    }.ToFrozenDictionary();
}