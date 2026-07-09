using System.Collections.Frozen;
using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.RateLimits;

public static class BucketDefaults
{
    public static readonly FrozenDictionary<string, ConfigRateLimitBucket> Values = new Dictionary<string, ConfigRateLimitBucket>()
    {
        // TODO: consider whether deletion endpoints should even be rate-limited if all they do is delete an entity.
        // TODO: also consider whether moderation endpoints should be rate-limited. I don't really think so, but maybe there could be a good reason for it?
        // TODO: maybe split these into multiple config files, one per endpoint category? (mostly for https://github.com/LittleBigRefresh/Refresh/issues/1099)
        // NOTE: Bucket names are prefixed by where the bucket is used (game/certain game/API/any)
#region Misc
        {BucketNames.Global, new(90, 380, 45)},
#endregion
        
#region Levels
        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {BucketNames.GameStartPublishLevel, new(900, 15, 600)},
        {BucketNames.GameFullyPublishLevel, new(900, 15, 600)},
        {BucketNames.GameGetCategoryLevelList, new(240, 60, 180)},
        // game likes to request many singular levels at times
        {BucketNames.GameGetSingleLevel, new(240, 260, 180)},
        {BucketNames.GameGetLevelsByIds, new(240, 30, 180)},
        
        {BucketNames.ApiEditLevel, new(300, 10, 180)},
        {BucketNames.ApiGetLevelsFromCategory, new(240, 60, 180)},
        {BucketNames.ApiGetSingleLevel, new(240, 60, 180)},
        
        {BucketNames.AnyDeleteLevel, new(300, 15, 180)},
        {BucketNames.AnyHeartLevel, new(300, 30, 180)},
        // This high because LBP3 has a feature where it will queue all levels in a playlist;
        // also people will generally probably queue a lot
        {BucketNames.AnyQueueLevel, new(300, 48, 180)},
        {BucketNames.AnyTagLevel, new(300, 10, 180)},
        {BucketNames.AnyRateLevel, new(300, 18, 180)},
        
        // PSP will sync level ratings which were done offline, and it'll refuse to login if syncing these fails
        {BucketNames.PspRateLevel, new(300, 90, 180)},
#endregion
        
#region Level Scores
        {BucketNames.GamePlayLevel, new(300, 40, 180)},
        {BucketNames.GameSubmitLevelScore, new(300, 30, 180)},
        {BucketNames.GameGetLevelScores, new(300, 70, 180)},
        
        // Same deal as with PSP level rating above, except play isn't as brutal because PSP uses a count query param
        // to avoid too many requests on that one endpoint atleast
        {BucketNames.PspPlayLevel, new(300, 90, 180)},
        {BucketNames.PspSubmitLevelScore, new(300, 90, 180)},
        // PSP spams these requests for story levels
        {BucketNames.PspGetLevelScores, new(300, 210, 180)},
        
        {BucketNames.ApiGetLevelScores, new(300, 40, 180)},
#endregion
        
#region Reviews
        {BucketNames.GameGetReviews, new(300, 60, 180)},
        
        {BucketNames.ApiGetReviews, new(300, 60, 180)},
        
        {BucketNames.AnySubmitReview, new(300, 12, 180)},
        {BucketNames.AnyRateReviews, new(300, 40, 180)},
        {BucketNames.AnyDeleteReviews, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {BucketNames.GameGetComments, new(300, 60, 180)},
        
        {BucketNames.ApiGetComments, new(300, 60, 180)},
        
        {BucketNames.AnySubmitComment, new(300, 18, 180)},
        {BucketNames.AnyRateComments, new(300, 40, 180)},
        {BucketNames.AnyDeleteComments, new(300, 30, 180)},
#endregion
        
#region Photos
        {BucketNames.GameUploadPhoto, new(300, 25, 180)},
        {BucketNames.GameGetPhotos, new(300, 60, 180)},
        {BucketNames.GameGetSinglePhoto, new(300, 30, 180)},
        
        {BucketNames.ApiGetPhotos, new(300, 60, 180)},
        {BucketNames.ApiGetSinglePhoto, new(300, 30, 180)},
        
        {BucketNames.AnyDeletePhotos, new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        {BucketNames.GameUpdateUser, new(240, 20, 180)},
        {BucketNames.GameUpdateWebsitePrivacy, new(240, 10, 180)},
        {BucketNames.GameGetUsersFromCategory, new(240, 60, 180)},
        {BucketNames.GameGetSingleUser, new(240, 60, 180)},
        {BucketNames.GameGetUsersByNames, new(240, 30, 180)},
        {BucketNames.GameUploadNpUserData, new(240, 6, 180)},
        
        {BucketNames.ApiUpdateUser, new(240, 20, 180)},
        // This one requires double authentication so it needs to be rate-limited regardless
        {BucketNames.ApiDeleteOwnUser, new(600, 6, 480)},
        {BucketNames.ApiGetUsersFromCategory, new(240, 60, 180)},
        {BucketNames.ApiGetSingleUser, new(240, 60, 180)},
        
        {BucketNames.AnyHeartUser, new(300, 30, 180)},
#endregion
        
#region Moderation
        {BucketNames.GameUploadGriefReport, new(400, 10, 240)},
        {BucketNames.GameFilterModeratedAssetList, new(300, 60, 180)},
        // all because of adventure uploading
        {BucketNames.GameFilterMessage, new(20, 900, 10)},
#endregion
        
#region Assets
        {BucketNames.GameUploadAsset, new(300, 150, 180)},
        {BucketNames.GameDownloadAsset, new(240, 280, 120)},
        
        {BucketNames.ApiUploadImage, new(300, 20, 180)},
        {BucketNames.ApiDownloadAsset, new(240, 200, 120)},
        {BucketNames.ApiDownloadImage, new(240, 200, 120)},
#endregion
        
#region Matching
        {BucketNames.GameRoomRequest, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {BucketNames.ApiGetRooms, new(240, 90, 120)},
        {BucketNames.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {BucketNames.Lbp1GetPlaylists, new(240, 40, 180)},
        {BucketNames.Lbp1GetSlotsFromPlaylist, new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general
        {BucketNames.Lbp3GetLevelsFromPlaylist, new(240, 90, 180)},
        {BucketNames.Lbp3GetPlaylistsByUser, new(240, 50, 180)},
        
        {BucketNames.ApiGetPlaylistsFromCategory, new(240, 60, 180)},
        {BucketNames.ApiGetSinglePlaylist, new(240, 40, 180)},
        
        {BucketNames.AnyCreatePlaylist, new(240, 40, 180)},
        {BucketNames.AnyUpdatePlaylist, new(240, 50, 180)},
        {BucketNames.AnyHeartPlaylist, new(240, 30, 180)},
        {BucketNames.AnyDeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity + Notifications
        {BucketNames.GameGetActivity, new(240, 50, 180)},
        {BucketNames.GameGetNotifications, new(240, 10, 180)},
        
        {BucketNames.ApiGetActivity, new(240, 50, 180)},
        {BucketNames.ApiGetNotifications, new(240, 30, 180)},
        {BucketNames.ApiGetSingleNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if a request to get Genre categories fails
        {BucketNames.GameGetCategories, new(240, 40, 180)},
        
        {BucketNames.GameGetLevelCategories, new(240, 25, 180)},
        {BucketNames.GameGetUserCategories, new(240, 25, 180)},
#endregion
        
#region Instance
        {BucketNames.GameGetConfig, new(240, 30, 180)},
        {BucketNames.GameGetInstanceStats, new(240, 15, 180)},
        {BucketNames.GameGetEula, new(240, 15, 180)},
        {BucketNames.GameGetAnnouncements, new(240, 15, 180)},
        
        {BucketNames.ApiGetInstanceInfo, new(240, 20, 180)},
        {BucketNames.ApiGetInstanceStats, new(240, 20, 180)},
        {BucketNames.ApiGetAnnouncements, new(240, 15, 180)},
#endregion
        
#region Pins
        {BucketNames.GameSyncPins, new(240, 12, 180)},
#endregion
        
#region Challenges
        {BucketNames.GameUploadChallenge, new(240, 8, 180)},
        {BucketNames.GameUploadChallengeScore, new(240, 16, 180)},
        
        {BucketNames.GameGetChallenges, new(240, 20, 180)},
        {BucketNames.GameGetChallengeScores, new(240, 50, 180)},
        {BucketNames.GameGetSingleChallengeScore, new(240, 40, 180)},
#endregion
        
        // TODO finish the list
    }.ToFrozenDictionary();
    
    /// <summary>
    /// Maps bucket names of game endpoints to PSP-specific ones, as PSP uses the same game endpoints as the other mainlines (or just LBP1)
    /// </summary>
    public static readonly FrozenDictionary<string, string> PspNameOverrides = new Dictionary<string, string>()
    {
        {BucketNames.AnyRateLevel, BucketNames.PspRateLevel},
        {BucketNames.GamePlayLevel, BucketNames.PspPlayLevel},
        {BucketNames.GameSubmitLevelScore, BucketNames.PspSubmitLevelScore},
        {BucketNames.GameGetLevelScores, BucketNames.PspGetLevelScores},
    }.ToFrozenDictionary();
}