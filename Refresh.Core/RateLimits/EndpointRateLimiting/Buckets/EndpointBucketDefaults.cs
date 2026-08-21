using System.Collections.Frozen;
using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

// We should try to have these defaults be rather lenient than too aggressive, since the latter would be far more negatively impactful
// and much more prone to rate-limit regular users who aren't really spamming.
// We shouldn't only rely on rate-limiting, so TODO: improve caching and generally optimize request handling, especially fetch requests
public static class EndpointBucketDefaults
{
    public static readonly FrozenDictionary<EndpointBucketName, ConfigRateLimitBucket> Buckets = new Dictionary<EndpointBucketName, ConfigRateLimitBucket>()
    {
#region Misc
        {EndpointBucketName.Default, new(90, 380, 45)},
#endregion
        
#region Levels
        {EndpointBucketName.GameGetListOfLevels, new(240, 50, 180)},
        {EndpointBucketName.GameGetSingleLevel, new(240, 200, 180)}, // game sometimes requests many levels in bursts
        
        {EndpointBucketName.ApiGetListOfLevels, new(240, 50, 180)},
        {EndpointBucketName.ApiGetSingleLevel, new(240, 50, 180)},
        {EndpointBucketName.ApiGetOwnRelationsToLevel, new(240, 50, 180)},
        
        {EndpointBucketName.GamePrepareLevelPublish, new(600, 20, 360)},
        {EndpointBucketName.GameRealLevelPublish, new(600, 20, 360)}, // both should be the same since they're always called together
        {EndpointBucketName.ApiEditLevel, new(300, 20, 180)},
        
        {EndpointBucketName.DeleteLevel, new(300, 20, 180)},
        {EndpointBucketName.HeartLevel, new(300, 30, 180)},
        {EndpointBucketName.QueueLevel, new(300, 50, 180)}, // lbp3 has a hacky feature where you can mass-queue levels from playlists
        {EndpointBucketName.TagLevel, new(300, 10, 180)},
        {EndpointBucketName.RateLevel, new(300, 20, 180)},
        
        // psp saves should have an average of 50 or so levels (depends on "Download Moon")
        // allow higher max request count at the cost of higher window/block duration
        {EndpointBucketName.PspRateLevel, new(420, 90, 300)}, 
#endregion
        
#region Level Scores
        {EndpointBucketName.GamePlayLevel, new(300, 30, 180)},
        {EndpointBucketName.PspPlayLevel, new(420, 90, 300)}, // see comment on PspRateLevel
        {EndpointBucketName.GameUploadLevelScore, new(300, 30, 180)},
        {EndpointBucketName.PspUploadLevelScore, new(420, 90, 300)},
        
        // PSP spams these requests for story levels every time the download moon is loaded, apparently.
        {EndpointBucketName.PspGetListOfLevelScores, new(300, 150, 180)},
        {EndpointBucketName.GameGetListOfLevelScores, new(300, 40, 180)},
        {EndpointBucketName.ApiGetListOfLevelScores, new(300, 40, 180)},
        {EndpointBucketName.ApiGetSingleLevelScore, new(300, 40, 180)},
#endregion
        
#region Reviews
        {EndpointBucketName.ApiGetListOfReviews, new(300, 40, 180)},
        {EndpointBucketName.ApiGetSingleReview, new(300, 40, 180)},
        
        {EndpointBucketName.GameGetListOfReviews, new(300, 40, 180)},
        {EndpointBucketName.GameGetSingleReview, new(300, 40, 180)},
        
        {EndpointBucketName.UploadReview, new(300, 12, 180)},
        {EndpointBucketName.RateReview, new(300, 40, 180)},
        {EndpointBucketName.DeleteReview, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {EndpointBucketName.ApiGetListOfComments, new(300, 40, 180)},
        {EndpointBucketName.ApiGetSingleComment, new(300, 40, 180)},
        
        {EndpointBucketName.GameGetListOfComments, new(300, 40, 180)},
        {EndpointBucketName.GameGetSingleComment, new(300, 40, 180)},
        
        {EndpointBucketName.UploadComment, new(300, 18, 180)},
        {EndpointBucketName.RateComment, new(300, 40, 180)},
        {EndpointBucketName.DeleteComment, new(300, 30, 180)},
#endregion
        
#region Photos
        {EndpointBucketName.GameGetListOfPhotos, new(300, 40, 180)},
        {EndpointBucketName.GameGetSinglePhoto, new(300, 40, 180)},
        
        {EndpointBucketName.ApiGetListOfPhotos, new(300, 40, 180)},
        {EndpointBucketName.ApiGetSinglePhoto, new(300, 40, 180)},
        
        {EndpointBucketName.GameUploadPhoto, new(300, 25, 180)},
        {EndpointBucketName.DeletePhoto, new(300, 30, 180)},
#endregion
        
#region Users
        {EndpointBucketName.GameGetListOfUsers, new(300, 60, 180)},
        {EndpointBucketName.GameGetSingleUser, new(300, 60, 180)},
        
        {EndpointBucketName.ApiGetListOfUsers, new(300, 60, 180)},
        {EndpointBucketName.ApiGetSingleUser, new(300, 60, 180)},
        
        {EndpointBucketName.UpdateUser, new(300, 20, 180)},
        {EndpointBucketName.HeartUser, new(300, 30, 180)},
        {EndpointBucketName.GameUploadFriendData, new(240, 6, 180)},
        
#endregion
        
#region Moderation
        {EndpointBucketName.GameUploadGriefReport, new(300, 10, 240)},
        {EndpointBucketName.GameFilterModeratedAssets, new(300, 60, 180)},
        // all because of adventure uploading, TODO rate-limit specific chat commands separately
        {EndpointBucketName.GameFilterChatMessage, new(60, 900, 30)},
#endregion
        
#region Assets
        // Regular download limits are this high on both game and API because both the game and third party API clients
        // (e.g. archive_dl) are likely to download many of these at times depending on what level they're trying to load
        // (additionally, adventures can have even more dependencies!)
        {EndpointBucketName.GameUploadAsset, new(300, 150, 180)},
        {EndpointBucketName.GameDownloadAsset, new(240, 500, 120)},
        
        {EndpointBucketName.ApiUploadImage, new(300, 20, 180)},
        {EndpointBucketName.ApiDownloadAsset, new(240, 500, 120)},
        {EndpointBucketName.ApiDownloadImage, new(240, 250, 120)},
        
        {EndpointBucketName.ApiGetAssetMetadata, new(240, 250, 120)},
#endregion
        
#region Matching
        {EndpointBucketName.GameUpdateRoomOrGetRooms, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating, which we have to cope with for now
        {EndpointBucketName.ApiGetListOfRooms, new(240, 90, 120)},
        {EndpointBucketName.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {EndpointBucketName.GameGetListOfPlaylists, new(240, 50, 180)},
        {EndpointBucketName.GameGetPlaylistContents, new(240, 50, 180)},
        
        // LBP3 doesn't cache these at all, and is inefficient with them in general, so we need less lenient rate-limits
        {EndpointBucketName.Lbp3GetListOfPlaylists, new(240, 50, 180)},
        {EndpointBucketName.Lbp3GetPlaylistContents, new(240, 90, 180)},
        
        {EndpointBucketName.ApiGetListOfPlaylists, new(240, 50, 180)},
        {EndpointBucketName.ApiGetSinglePlaylist, new(240, 50, 180)},
        
        {EndpointBucketName.CreatePlaylist, new(240, 30, 180)},
        {EndpointBucketName.UpdatePlaylistMetadata, new(240, 30, 180)},
        {EndpointBucketName.UpdatePlaylistContents, new(240, 50, 180)},
        {EndpointBucketName.HeartPlaylist, new(240, 30, 180)},
        {EndpointBucketName.DeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity
        {EndpointBucketName.GameGetActivityPage, new(240, 50, 180)},
        {EndpointBucketName.ApiGetActivityPage, new(240, 50, 180)},
#endregion

#region Notifications
        {EndpointBucketName.GameGetListOfNotifications, new(240, 20, 180)},
        
        {EndpointBucketName.ApiGetListOfNotifications, new(240, 20, 180)},
        {EndpointBucketName.ApiGetSingleNotification, new(240, 20, 180)},
        {EndpointBucketName.ApiDeleteNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if fetching Genre categories fails
        {EndpointBucketName.GameGetListOfCategories, new(240, 40, 180)},
        {EndpointBucketName.ApiGetListOfCategories, new(240, 20, 180)},
#endregion
        
#region Instance
        {EndpointBucketName.GameGetGameConfig, new(240, 30, 180)},
        {EndpointBucketName.GameGetInstanceStats, new(240, 30, 180)},
        {EndpointBucketName.GameGetEula, new(240, 30, 180)},
        {EndpointBucketName.GameGetListOfAnnouncements, new(240, 30, 180)},
        
        {EndpointBucketName.ApiGetInstanceInfo, new(240, 30, 180)},
        {EndpointBucketName.ApiGetInstanceStats, new(240, 30, 180)},
        {EndpointBucketName.ApiGetDocumentation, new(240, 30, 180)},
        {EndpointBucketName.ApiGetListOfAnnouncements, new(240, 30, 180)},
#endregion
        
#region Pins
        {EndpointBucketName.GameSyncPinProgress, new(240, 12, 180)},
#endregion
        
#region Challenges
        {EndpointBucketName.GameUploadPlayerChallenge, new(240, 8, 180)},
        {EndpointBucketName.GameUploadPlayerChallengeScore, new(240, 16, 180)},
        
        {EndpointBucketName.GameGetListOfPlayerChallenges, new(240, 20, 180)},
        {EndpointBucketName.GameGetListOfPlayerChallengeScores, new(240, 50, 180)},
        {EndpointBucketName.GameGetSinglePlayerChallengeScore, new(240, 40, 180)},
#endregion
        
#region Authentication
    {EndpointBucketName.GameLogin, new(300, 10, 300)},
    {EndpointBucketName.ApiLogin, new(300, 10, 300)},
    {EndpointBucketName.ApiRegister, new(3600, 10, 1800)},

    {EndpointBucketName.ApiRequestEmail, new(300, 10, 300)},
    {EndpointBucketName.ApiVerifyEmailAddress, new(300, 10, 300)},
    {EndpointBucketName.ApiResetPassword, new(300, 10, 300)},
    
    {EndpointBucketName.ApiGetListOfIpAddresses, new(300, 30, 240)},
    {EndpointBucketName.ApiApproveOrDenyIpAddress, new(300, 30, 240)},
    
    {EndpointBucketName.ApiDeleteOwnUser, new(600, 6, 480)},
#endregion
    }.ToFrozenDictionary();
}