using System.Collections.Frozen;
using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits;

public static class GameEndpointBucketDefaults
{
    // TODO split into multiple dictionaries to not overwhelm other instance owners as much, but what should they be grouped by?
    // game/API/any? category?
    public static readonly FrozenDictionary<BucketName, ConfigRateLimitBucket> Defaults = new Dictionary<BucketName, ConfigRateLimitBucket>()
    {
#region Misc
        {BucketName.Default, new(90, 380, 45)},
#endregion

// Some level-related rate-limits have special params for LBP PSP because, on login, PSP will upload all scores, plays, and ratings done offline,
// which will result in larger bursts of requests. Responding with a non-OK status to any of these will make PSP fail to login.
// We can have a little over 90 max request counts for such endpoints for PSP because that's probably how many downloaded levels
// users will have at the same time on average.
        
#region Levels
        {BucketName.GetLevels, new(240, 30, 180)},
        {BucketName.GetLevelsByListOfIds, new(240, 30, 180)},
        {BucketName.ApiGetSingleLevel, new(240, 60, 180)},
        // game is special-cased because in some cases, the game wants to request many separately at once
        {BucketName.GameGetSingleLevel, new(240, 260, 180)},

        // These two below are separate so we don't run into cases where /startPublish doesn't rate-limit but /publish does
        {BucketName.PrepareLevelPublish, new(900, 15, 600)},
        {BucketName.RealLevelPublish, new(900, 15, 600)},
        {BucketName.ApiEditLevel, new(300, 20, 180)},
        
        {BucketName.DeleteLevel, new(300, 20, 180)},
        {BucketName.HeartLevel, new(300, 30, 180)},
        {BucketName.QueueLevel, new(300, 50, 180)},
        {BucketName.TagLevel, new(300, 10, 180)},
        {BucketName.RateLevel, new(300, 20, 180)},
        
        {BucketName.PspRateLevel, new(420, 90, 240)},
#endregion
        
#region Level Scores
        {BucketName.PlayLevel, new(300, 40, 180)},
        {BucketName.UploadLevelScore, new(300, 30, 180)},
        {BucketName.GetLevelScores, new(300, 70, 180)},
        
        {BucketName.PspPlayLevel, new(300, 30, 180)},
        {BucketName.PspUploadLevelScore, new(300, 30, 180)},
        // PSP spams these requests for story levels every time the download moon is loaded, apparently.
        {BucketName.PspGetLevelScores, new(300, 150, 180)},
#endregion
        
#region Reviews
        {BucketName.GetReviews, new(300, 60, 180)},
        {BucketName.GetSingleReview, new(300, 30, 180)},
        
        {BucketName.UploadReview, new(300, 12, 180)},
        {BucketName.RateReview, new(300, 40, 180)},
        {BucketName.DeleteReview, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {BucketName.GetComments, new(300, 60, 180)},
        
        {BucketName.UploadComment, new(300, 18, 180)},
        {BucketName.RateComment, new(300, 40, 180)},
        {BucketName.DeleteComment, new(300, 30, 180)},
#endregion
        
#region Photos
        {BucketName.GetPhotos, new(300, 60, 180)},
        {BucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.GetPhotos, new(300, 60, 180)},
        {BucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {BucketName.UploadPhoto, new(300, 25, 180)},
        {BucketName.DeletePhoto, new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        
        {BucketName.GetUsers, new(240, 60, 180)},
        {BucketName.GetSingleUser, new(240, 60, 180)},
        {BucketName.GetUsersByListOfNames, new(240, 30, 180)},
        
        {BucketName.UpdateUser, new(240, 20, 180)},
        {BucketName.HeartUser, new(300, 30, 180)},
        {BucketName.UploadFriendData, new(240, 6, 180)},
        {BucketName.DeleteOwnUser, new(600, 6, 480)},
#endregion
        
#region Moderation
        {BucketName.UploadGriefReport, new(400, 10, 240)},
        {BucketName.FilterModeratedAssets, new(300, 60, 180)},
        // all because of adventure uploading
        {BucketName.FilterChatMessage, new(20, 900, 10)},
#endregion
        
#region Assets
        // Regular download limits are this high on both game and API because both the game and third party API clients
        // (e.g. archive_dl) are likely to download many of these at times depending on what level they're trying to load
        // (additionally, adventures can have even more dependencies!)
        {BucketName.GameUploadAsset, new(300, 150, 180)},
        {BucketName.GameDownloadAsset, new(240, 500, 120)},
        
        {BucketName.ApiUploadImage, new(300, 20, 180)},
        {BucketName.ApiDownloadAsset, new(240, 500, 120)},
        {BucketName.ApiDownloadImage, new(240, 250, 120)},
        
        {BucketName.ApiGetAssetInfo, new(240, 250, 120)},
#endregion
        
#region Matching
        {BucketName.GameUpdateRoomOrGetRooms, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {BucketName.ApiGetRooms, new(240, 90, 120)},
        {BucketName.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {BucketName.GetPlaylists, new(240, 40, 180)},
        {BucketName.GetLevelsFromPlaylist, new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general, so we need special-cases
        {BucketName.Lbp3GetPlaylists, new(240, 50, 180)},
        {BucketName.Lbp3GetLevelsFromPlaylist, new(240, 90, 180)},
        
        {BucketName.GetSinglePlaylist, new(240, 40, 180)},
        
        {BucketName.CreatePlaylist, new(240, 40, 180)},
        {BucketName.UpdatePlaylist, new(240, 50, 180)},
        {BucketName.HeartPlaylist, new(240, 30, 180)},
        {BucketName.DeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity
        {BucketName.GetActivityPage, new(240, 50, 180)},
#endregion

#region Notifications
        {BucketName.GetNotifications, new(240, 15, 180)},
        {BucketName.GetSingleNotification, new(240, 20, 180)},
        {BucketName.DeleteNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if fetching Genre categories fails
        {BucketName.GetCategories, new(240, 40, 180)},
#endregion
        
#region Instance
        {BucketName.GetGameConfig, new(240, 30, 180)},
        {BucketName.GetInstanceInfo, new(240, 20, 180)},
        {BucketName.GetInstanceStats, new(240, 15, 180)},
        {BucketName.GetEula, new(240, 15, 180)},
        {BucketName.GetAnnouncements, new(240, 15, 180)},
#endregion
        
#region Pins
        {BucketName.SyncPinProgress, new(240, 12, 180)},
#endregion
        
#region Challenges
        {BucketName.UploadPlayerChallenge, new(240, 8, 180)},
        {BucketName.UploadPlayerChallengeScore, new(240, 16, 180)},
        
        {BucketName.GetPlayerChallenges, new(240, 20, 180)},
        {BucketName.GetPlayerChallengeScores, new(240, 50, 180)},
        {BucketName.GetSinglePlayerChallengeScore, new(240, 40, 180)},
#endregion
        
#region Authentication
    {BucketName.GameLogin, new(300, 10, 300)},
    {BucketName.ApiLogin, new(300, 10, 300)},
    {BucketName.Register, new(3600, 10, 1800)},
    {BucketName.RefreshToken, new(300, 10, 300)},

    {BucketName.SendEmail, new(300, 10, 300)},
    {BucketName.VerifyEmailAddress, new(300, 10, 300)},
    {BucketName.ResetPassword, new(300, 10, 300)},
#endregion
    }.ToFrozenDictionary();
}