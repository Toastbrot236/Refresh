using System.Collections.Frozen;
using Refresh.Core.Configuration.Structs;

namespace Refresh.Core.RateLimits;

public static class BucketDefaults
{
    public static readonly FrozenDictionary<BucketName, ConfigRateLimitBucket> Values = new Dictionary<BucketName, ConfigRateLimitBucket>()
    {
        // TODO: consider whether deletion endpoints should even be rate-limited if all they do is delete an entity.
        // TODO: also consider whether moderation endpoints should be rate-limited. I don't really think so, but maybe there could be a good reason for it?
        // TODO: maybe split these into multiple config files, one per endpoint category? (mostly for https://github.com/LittleBigRefresh/Refresh/issues/1099)
        // NOTE: Bucket names are prefixed by where the bucket is used (game/certain game/API/any)
#region Misc
        {BucketName.Global, new(90, 380, 45)},
#endregion
        
#region Levels
        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {BucketName.GameStartPublishLevel, new(900, 15, 600)},
        {BucketName.GameFullyPublishLevel, new(900, 15, 600)},
        {BucketName.GameGetCategoryLevelList, new(240, 60, 180)},
        // game likes to request many singular levels at times
        {BucketName.GameGetSingleLevel, new(240, 260, 180)},
        {BucketName.GameGetLevelsByIds, new(240, 30, 180)},
        
        {BucketName.ApiEditLevel, new(300, 10, 180)},
        {BucketName.ApiGetLevelsFromCategory, new(240, 60, 180)},
        {BucketName.ApiGetSingleLevel, new(240, 60, 180)},
        
        {BucketName.AnyDeleteLevel, new(300, 15, 180)},
        {BucketName.AnyHeartLevel, new(300, 30, 180)},
        // This high because LBP3 has a feature where it will queue all levels in a playlist;
        // also people will generally probably queue a lot
        {BucketName.AnyQueueLevel, new(300, 48, 180)},
        {BucketName.AnyTagLevel, new(300, 10, 180)},
        {BucketName.AnyRateLevel, new(300, 18, 180)},
        
        // PSP will sync level ratings which were done offline, and it'll refuse to login if syncing these fails
        {BucketName.PspRateLevel, new(300, 90, 180)},
#endregion
        
#region Level Scores
        {BucketName.GamePlayLevel, new(300, 40, 180)},
        {BucketName.GameSubmitLevelScore, new(300, 30, 180)},
        {BucketName.GameGetLevelScores, new(300, 70, 180)},
        
        // Same deal as with PSP level rating above, except play isn't as brutal because PSP uses a count query param
        // to avoid too many requests on that one endpoint atleast
        {BucketName.PspPlayLevel, new(300, 90, 180)},
        {BucketName.PspSubmitLevelScore, new(300, 90, 180)},
        // PSP spams these requests for story levels
        {BucketName.PspGetLevelScores, new(300, 210, 180)},
        
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
        {BucketName.AnyRateComments, new(300, 40, 180)},
        {BucketName.AnyDeleteComments, new(300, 30, 180)},
#endregion
        
#region Photos
        {BucketName.GameUploadPhoto, new(300, 25, 180)},
        {BucketName.GameGetPhotos, new(300, 60, 180)},
        {BucketName.GameGetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.ApiGetPhotos, new(300, 60, 180)},
        {BucketName.ApiGetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.AnyDeletePhotos, new(300, 30, 180)},
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
        
        {BucketName.AnyHeartUser, new(300, 30, 180)},
#endregion
        
#region Moderation
        {BucketName.GameUploadGriefReport, new(400, 10, 240)},
        {BucketName.GameFilterModeratedAssetList, new(300, 60, 180)},
        // all because of adventure uploading
        {BucketName.GameFilterMessage, new(20, 900, 10)},
#endregion
        
#region Assets
        {BucketName.GameUploadAsset, new(300, 150, 180)},
        {BucketName.GameDownloadAsset, new(240, 280, 120)},
        
        {BucketName.ApiUploadImage, new(300, 20, 180)},
        {BucketName.ApiDownloadAsset, new(240, 200, 120)},
        {BucketName.ApiDownloadImage, new(240, 200, 120)},
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
        {BucketName.Lbp3GetPlaylistsByUser, new(240, 50, 180)},
        
        {BucketName.ApiGetPlaylistsFromCategory, new(240, 60, 180)},
        {BucketName.ApiGetSinglePlaylist, new(240, 40, 180)},
        
        {BucketName.AnyCreatePlaylist, new(240, 40, 180)},
        {BucketName.AnyUpdatePlaylist, new(240, 50, 180)},
        {BucketName.AnyHeartPlaylist, new(240, 30, 180)},
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
    /// Maps bucket names of game endpoints to PSP-specific ones, as PSP uses the same game endpoints as the other mainlines (or just LBP1)
    /// </summary>
    public static readonly FrozenDictionary<BucketName, BucketName> PspNameOverrides = new Dictionary<BucketName, BucketName>()
    {
        {BucketName.AnyRateLevel, BucketName.PspRateLevel},
        {BucketName.GamePlayLevel, BucketName.PspPlayLevel},
        {BucketName.GameSubmitLevelScore, BucketName.PspSubmitLevelScore},
        {BucketName.GameGetLevelScores, BucketName.PspGetLevelScores},
    }.ToFrozenDictionary();
}