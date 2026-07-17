using System.Collections.Frozen;
using Bunkum.Core.Configuration;
using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.Configuration.RateLimitConfigs;

public class EndpointRateLimitConfig : Config
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
}