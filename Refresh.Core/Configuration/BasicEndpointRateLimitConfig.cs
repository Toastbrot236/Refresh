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
        // TODO: maybe split these into multiple config files, one per endpoint category? (mostly for https://github.com/LittleBigRefresh/Refresh/issues/1099)
        // NOTE: Bucket names are prefixed by where the bucket is used (game/certain game/API/any)
#region Misc
        {"global", new(90, 380, 45)},
#endregion
        
#region Levels
        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {"gameStartPublishLevel", new(900, 15, 600)},
        {"gameFullyPublishLevel", new(900, 15, 600)},
        {"gameGetCategoryLevelList", new(240, 60, 180)},
        // game likes to request many singular levels at times
        {"gameGetSingleLevel", new(240, 260, 180)},
        {"gameGetLevelsByIds", new(240, 30, 180)},
        
        {"apiEditLevel", new(300, 10, 180)},
        {"apiGetLevelsFromCategory", new(240, 60, 180)},
        {"apiGetSingleLevel", new(240, 60, 180)},
        
        {"anyDeleteLevel", new(300, 15, 180)},
        {"anyHeartLevel", new(300, 30, 180)},
        // This high because LBP3 has a feature where it will queue all levels in a playlist;
        // also people will generally probably queue a lot
        {"anyQueueLevel", new(300, 48, 180)},
        {"anyTagLevel", new(300, 10, 180)},
        {"anyRateLevel", new(300, 18, 180)},
        
        // PSP will sync level ratings which were done offline, and it'll refuse to login if syncing these fails
        {"pspRateLevel", new(300, 90, 180)},
#endregion
        
#region Level Scores
        {"gamePlayLevel", new(300, 40, 180)},
        {"gameSubmitLevelScore", new(300, 30, 180)},
        {"gameGetLevelScores", new(300, 70, 180)},
        
        // Same deal as with PSP level rating above, except play isn't as brutal because PSP uses a count query param
        // to avoid too many requests on that one endpoint atleast
        {"pspPlayLevel", new(300, 90, 180)},
        {"pspSubmitLevelScore", new(300, 90, 180)},
        // PSP spams these requests for story levels
        {"pspGetLevelScores", new(300, 210, 180)},
        
        {"apiGetLevelScores", new(300, 40, 180)},
#endregion
        
#region Reviews
        {"gameGetReviews", new(300, 60, 180)},
        
        {"apiGetReviews", new(300, 60, 180)},
        
        {"anySubmitReview", new(300, 12, 180)},
        {"anyRateReviews", new(300, 40, 180)},
        {"anyDeleteReviews", new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {"gameGetComments", new(300, 60, 180)},
        
        {"apiGetComments", new(300, 60, 180)},
        
        {"anySubmitComment", new(300, 18, 180)},
        {"anyRateComments", new(300, 40, 180)},
        {"anyDeleteComments", new(300, 30, 180)},
#endregion
        
#region Photos
        {"gameUploadPhoto", new(300, 25, 180)},
        {"gameGetPhotos", new(300, 60, 180)},
        {"gameGetSinglePhoto", new(300, 30, 180)},
        
        {"apiGetPhotos", new(300, 60, 180)},
        {"apiGetSinglePhoto", new(300, 30, 180)},
        
        {"anyDeletePhotos", new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        {"gameUpdateUser", new(240, 20, 180)},
        {"gameUpdateWebsitePrivacy", new(240, 10, 180)},
        {"gameGetUsersFromCategory", new(240, 60, 180)},
        {"gameGetSingleUser", new(240, 60, 180)},
        {"gameGetUsersByNames", new(240, 30, 180)},
        {"gameUploadNpUserData", new(240, 6, 180)},
        
        {"apiUpdateUser", new(240, 20, 180)},
        // This one requires double authentication so it needs to be rate-limited regardless
        {"apiDeleteOwnUser", new(600, 6, 480)},
        {"apiGetUsersFromCategory", new(240, 60, 180)},
        {"apiGetSingleUser", new(240, 60, 180)},
        
        {"anyHeartUser", new(300, 30, 180)},
#endregion
        
#region Moderation
        {"gameUploadGriefReport", new(400, 10, 240)},
        {"gameFilterModeratedAssetList", new(300, 60, 180)},
        // all because of adventure uploading
        {"gameFilterMessage", new(20, 900, 10)},
#endregion
        
#region Assets
        {"gameUploadAsset", new(300, 150, 180)},
        {"gameDownloadAsset", new(240, 280, 120)},
        
        {"apiUploadImage", new(300, 20, 180)},
        {"apiDownloadAsset", new(240, 200, 120)},
        {"apiDownloadImage", new(240, 200, 120)},
#endregion
        
#region Matching
        {"gameRoomRequest", new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {"apiGetRooms", new(240, 90, 120)},
        {"apiGetSingleRoom", new(240, 40, 120)},
#endregion
        
#region Playlists
        {"lbp1GetPlaylists", new(240, 40, 180)},
        {"lbp1GetSlotsFromPlaylist", new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general
        {"lbp3GetLevelsFromPlaylist", new(240, 90, 180)},
        {"lbp3GetPlaylistsByUser", new(240, 50, 180)},
        
        {"apiGetPlaylistsFromCategory", new(240, 60, 180)},
        {"apiGetSinglePlaylist", new(240, 40, 180)},
        
        {"anyCreatePlaylist", new(240, 40, 180)},
        {"anyUpdatePlaylist", new(240, 50, 180)},
        {"anyHeartPlaylist", new(240, 30, 180)},
        {"anyDeletePlaylist", new(240, 30, 180)},
#endregion
        
#region Activity
        {"gameGetActivity", new(240, 50, 180)},
        {"gameGetNotifications", new(240, 10, 180)},
        
        {"apiGetActivity", new(240, 50, 180)},
        {"apiGetNotifications", new(240, 30, 180)},
        {"apiGetSingleNotifications", new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if a request to get Genre categories fails
        {"gameGetCategories", new(240, 40, 180)},
        
        {"gameGetLevelCategories", new(240, 25, 180)},
        {"gameGetUserCategories", new(240, 25, 180)},
#endregion
        
#region Instance
        {"gameGetConfig", new(240, 30, 180)},
        {"gameGetInstanceStats", new(240, 15, 180)},
        {"gameGetEula", new(240, 15, 180)},
        {"gameGetAnnouncements", new(240, 15, 180)},
        
        {"apiGetInstanceInfo", new(240, 20, 180)},
        {"apiGetInstanceStats", new(240, 20, 180)},
        {"apiGetAnnouncements", new(240, 15, 180)},
#endregion
        
#region Pins
        {"gameSyncPins", new(240, 12, 180)},
#endregion
        
#region Challenges
        {"gameUploadChallenge", new(240, 8, 180)},
        {"gameUploadChallengeScore", new(240, 16, 180)},
        
        {"gameGetChallenges", new(240, 20, 180)},
        {"gameGetChallengeScores", new(240, 50, 180)},
        {"gameGetSingleChallengeScore", new(240, 40, 180)},
#endregion
        
        // TODO finish the list
    }.ToFrozenDictionary();
}