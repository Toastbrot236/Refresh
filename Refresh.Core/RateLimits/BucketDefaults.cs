using System.Collections.Frozen;
using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.RateLimits;

// Names are prefixed by where the bucket is used (game/API/any as in anywhere else)
// We usually try to have modifying endpoints (creation/update/deletion) share one bucket each, no matter if game/API,
// while having GET endpoints (lists, singular entities etc.) have separate buckets from game/API.
public static class BucketDefaults
{
    // I'd say 90 is a good max count for PSP because, while I don't know for sure, I hope that PSP's "download moon" has about as many craters,
    // and therefore as many downloaded levels it can hold per save, as "my moon".
    private const int PspMaxDownloadedLevelsCount = 90;
    private const int PspStoryLevelsCount = 60;
    private const int DefaultMaxHeartCount = 30;
    
    public static readonly FrozenDictionary<BucketName, ConfigRateLimitBucket> Values = new Dictionary<BucketName, ConfigRateLimitBucket>()
    {
        // TODO: consider whether deletion endpoints should even be rate-limited if all they do is delete an entity.
        // TODO: also consider whether moderation endpoints should be rate-limited. I don't really think so, but maybe there could be a good reason for it?
        // TODO: maybe split these into multiple config files, one per endpoint category? (mostly for https://github.com/LittleBigRefresh/Refresh/issues/1099)
#region Misc
        {BucketName.Global, new(90, 380, 45)},
#endregion
        
#region Levels
        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {BucketName.GameStartPublishLevel, new(900, 15, 600)},
        {BucketName.GameFullyPublishLevel, new(900, 15, 600)},
        {BucketName.GameGetLevelsFromCategory, new(240, 60, 180)},
        // game likes to request many singular levels at times
        {BucketName.GameGetSingleLevel, new(240, 260, 180)},
        {BucketName.GameGetLevelsByIds, new(240, 30, 180)},
        
        {BucketName.ApiEditLevel, new(300, 10, 180)},
        {BucketName.ApiGetLevelsFromCategory, new(240, 60, 180)},
        {BucketName.ApiGetSingleLevel, new(240, 60, 180)},
        
        {BucketName.AnyDeleteLevel, new(300, 15, 180)},
        {BucketName.AnyHeartLevel, new(300, DefaultMaxHeartCount, 180)},
        // Queueing is this high because LBP3 has a feature where it will queue all levels in a playlist;
        // also people might generally probably queue a lot.
        {BucketName.AnyQueueLevel, new(300, 56, 180)},
        {BucketName.AnyTagLevel, new(300, 10, 180)},
        {BucketName.AnyRateLevel, new(300, 18, 180)},
        
        // PSP will sync level ratings which were done offline, and it'll refuse to login if syncing these fails (if it receives an error status),
        // so special-case it.
        {BucketName.PspRateLevel, new(420, PspMaxDownloadedLevelsCount, 240)},
#endregion
        
#region Level Scores
        {BucketName.GamePlayLevel, new(300, 40, 180)},
        {BucketName.GameSubmitLevelScore, new(300, 30, 180)},
        {BucketName.GameGetLevelScores, new(300, 70, 180)},
        
        // Same deal as with PSP level rating above, except /play isn't as brutal because PSP uses a count query param
        // to avoid too many requests on that one endpoint at least.
        {BucketName.PspPlayLevel, new(300, PspMaxDownloadedLevelsCount, 180)},
        {BucketName.PspSubmitLevelScore, new(300, PspMaxDownloadedLevelsCount, 180)},
        // PSP spams these requests for story levels every time the download moon is loaded, apparently.
        {BucketName.PspGetLevelScores, new(300, PspStoryLevelsCount * 4, 180)},
        
        {BucketName.ApiGetLevelScores, new(300, 40, 180)},
#endregion
        
#region Reviews
        {BucketName.GameGetReviews, new(300, 60, 180)},
        
        {BucketName.ApiGetReviews, new(300, 60, 180)},
        
        {BucketName.AnySubmitReview, new(300, 12, 180)},
        {BucketName.AnyRateReviews, new(300, 40, 180)},
        {BucketName.AnyDeleteReviews, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {BucketName.GameGetComments, new(300, 60, 180)},
        
        {BucketName.ApiGetComments, new(300, 60, 180)},
        
        {BucketName.AnySubmitComment, new(300, 18, 180)},
        {BucketName.AnyRateComment, new(300, 40, 180)},
        {BucketName.AnyDeleteComment, new(300, 30, 180)},
#endregion
        
#region Photos
        {BucketName.GameUploadPhoto, new(300, 25, 180)},
        {BucketName.GameGetPhotos, new(300, 60, 180)},
        {BucketName.GameGetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.ApiGetPhotos, new(300, 60, 180)},
        {BucketName.ApiGetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.AnyDeletePhoto, new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        {BucketName.GameUpdateUser, new(240, 20, 180)},
        {BucketName.GameUpdateWebsitePrivacy, new(240, 10, 180)},
        {BucketName.GameGetUsersFromCategory, new(240, 60, 180)},
        {BucketName.GameGetSingleUser, new(240, 60, 180)},
        {BucketName.GameGetUsersByNames, new(240, 30, 180)},
        {BucketName.GameUploadNpUserData, new(240, 6, 180)},
        
        {BucketName.ApiUpdateUser, new(240, 20, 180)},
        // This one requires double authentication so it needs to be rate-limited regardless
        {BucketName.ApiDeleteOwnUser, new(600, 6, 480)},
        {BucketName.ApiGetUsersFromCategory, new(240, 60, 180)},
        {BucketName.ApiGetSingleUser, new(240, 60, 180)},
        
        {BucketName.AnyHeartUser, new(300, DefaultMaxHeartCount, 180)},
#endregion
        
#region Moderation
        {BucketName.GameUploadGriefReport, new(400, 10, 240)},
        {BucketName.GameFilterModeratedAssets, new(300, 60, 180)},
        // all because of adventure uploading
        {BucketName.GameFilterMessage, new(20, 900, 10)},
#endregion
        
#region Assets
        // Regular download limits are this high on both game and API because both the game and third party API clients
        // (e.g. archive_dl) are likely to download many of these at times depending on what level they're trying to load
        // (additionally, adventures can have even more dependencies!)
        {BucketName.GameUploadAsset, new(300, 150, 180)},
        {BucketName.GameDownloadAsset, new(240, 400, 120)},
        
        {BucketName.ApiUploadImage, new(300, 20, 180)},
        {BucketName.ApiDownloadAsset, new(240, 400, 120)},
        {BucketName.ApiDownloadImage, new(240, 250, 120)},
#endregion
        
#region Matching
        {BucketName.GameRoomRequest, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {BucketName.ApiGetRooms, new(240, 90, 120)},
        {BucketName.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {BucketName.Lbp1GetPlaylists, new(240, 40, 180)},
        {BucketName.Lbp1GetSlotsFromPlaylist, new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general
        {BucketName.Lbp3GetLevelsFromPlaylist, new(240, 90, 180)},
        {BucketName.Lbp3GetPlaylists, new(240, 50, 180)},
        
        {BucketName.ApiGetPlaylistsFromCategory, new(240, 60, 180)},
        {BucketName.ApiGetSinglePlaylist, new(240, 40, 180)},
        
        {BucketName.AnyCreatePlaylist, new(240, 40, 180)},
        {BucketName.AnyUpdatePlaylist, new(240, 50, 180)},
        {BucketName.AnyHeartPlaylist, new(240, DefaultMaxHeartCount, 180)},
        {BucketName.AnyDeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity + Notifications
        {BucketName.GameGetActivity, new(240, 50, 180)},
        {BucketName.GameGetNotifications, new(240, 10, 180)},
        
        {BucketName.ApiGetActivity, new(240, 50, 180)},
        {BucketName.ApiGetNotifications, new(240, 30, 180)},
        {BucketName.ApiGetSingleNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if a request to get Genre categories fails
        {BucketName.GameGetCategories, new(240, 40, 180)},
        
        {BucketName.GameGetLevelCategories, new(240, 25, 180)},
        {BucketName.GameGetUserCategories, new(240, 25, 180)},
#endregion
        
#region Instance
        {BucketName.GameGetConfig, new(240, 30, 180)},
        {BucketName.GameGetInstanceStats, new(240, 15, 180)},
        {BucketName.GameGetEula, new(240, 15, 180)},
        {BucketName.GameGetAnnouncements, new(240, 15, 180)},
        
        {BucketName.ApiGetInstanceInfo, new(240, 20, 180)},
        {BucketName.ApiGetInstanceStats, new(240, 20, 180)},
        {BucketName.ApiGetAnnouncements, new(240, 15, 180)},
#endregion
        
#region Pins
        {BucketName.GameSyncPins, new(240, 12, 180)},
#endregion
        
#region Challenges
        {BucketName.GameUploadChallenge, new(240, 8, 180)},
        {BucketName.GameUploadChallengeScore, new(240, 16, 180)},
        
        {BucketName.GameGetChallenges, new(240, 20, 180)},
        {BucketName.GameGetChallengeScores, new(240, 50, 180)},
        {BucketName.GameGetSingleChallengeScore, new(240, 40, 180)},
#endregion
        
        // TODO finish the list
    }.ToFrozenDictionary();
    
    /// <summary>
    /// Maps bucket names of game endpoints to PSP-specific ones, as PSP uses the same game endpoints as the other mainlines (or just LBP1).
    /// But since PSP is more quirky than the other games (considering how it handles failures and efficiency on some endpoints),
    /// we have to use way more lenient limits on such endpoints.
    /// </summary>
    public static readonly FrozenDictionary<BucketName, BucketName> PspNameOverrides = new Dictionary<BucketName, BucketName>()
    {
        {BucketName.AnyRateLevel, BucketName.PspRateLevel},
        {BucketName.GamePlayLevel, BucketName.PspPlayLevel},
        {BucketName.GameSubmitLevelScore, BucketName.PspSubmitLevelScore},
        {BucketName.GameGetLevelScores, BucketName.PspGetLevelScores},
    }.ToFrozenDictionary();
}