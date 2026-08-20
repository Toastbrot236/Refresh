using System.Collections.Frozen;
using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

public static class ApiEndpointBucketDefaults
{
    public static readonly FrozenDictionary<EndpointRateLimitBucket, ConfigRateLimitBucket> Buckets = new Dictionary<EndpointRateLimitBucket, ConfigRateLimitBucket>()
    {
#region Misc
        {EndpointBucketName.Default, new(90, 380, 45)},
#endregion
        
#region Levels
        {EndpointBucketName.GetListOfLevels, new(240, 50, 180)},
        {EndpointBucketName.GetSingleLevel, new(240, 50, 180)},
        {EndpointBucketName.GetOwnRelationsOnLevel, new(240, 50, 180)},
        
        {EndpointBucketName.EditLevel, new(300, 20, 180)},
        {EndpointBucketName.DeleteLevel, new(300, 20, 180)},
        {EndpointBucketName.HeartLevel, new(300, 30, 180)},
        {EndpointBucketName.QueueLevel, new(300, 50, 180)},
        {EndpointBucketName.TagLevel, new(300, 10, 180)},
        {EndpointBucketName.RateLevel, new(300, 20, 180)},
        
        {EndpointBucketName.PspRateLevel, new(420, 90, 240)},
#endregion
        
#region Level Scores
        {EndpointBucketName.PlayLevel, new(300, 40, 180)},
        {EndpointBucketName.UploadLevelScore, new(300, 30, 180)},
        {EndpointBucketName.GetLevelScores, new(300, 70, 180)},
        
        {EndpointBucketName.PspPlayLevel, new(300, 30, 180)},
        {EndpointBucketName.PspUploadLevelScore, new(300, 30, 180)},
        // PSP spams these requests for story levels every time the download moon is loaded, apparently.
        {EndpointBucketName.PspGetLevelScores, new(300, 150, 180)},
#endregion
        
#region Reviews
        {EndpointBucketName.GetReviews, new(300, 60, 180)},
        {EndpointBucketName.GetSingleReview, new(300, 30, 180)},
        
        {EndpointBucketName.UploadReview, new(300, 12, 180)},
        {EndpointBucketName.RateReview, new(300, 40, 180)},
        {EndpointBucketName.DeleteReview, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {EndpointBucketName.GetComments, new(300, 60, 180)},
        
        {EndpointBucketName.UploadComment, new(300, 18, 180)},
        {EndpointBucketName.RateComment, new(300, 40, 180)},
        {EndpointBucketName.DeleteComment, new(300, 30, 180)},
#endregion
        
#region Photos
        {EndpointBucketName.GetPhotos, new(300, 60, 180)},
        {EndpointBucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {EndpointBucketName.GetPhotos, new(300, 60, 180)},
        {EndpointBucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {EndpointBucketName.UploadPhoto, new(300, 25, 180)},
        {EndpointBucketName.DeletePhoto, new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        
        {EndpointBucketName.GetUsers, new(240, 60, 180)},
        {EndpointBucketName.GetSingleUser, new(240, 60, 180)},
        {EndpointBucketName.GetUsersByListOfNames, new(240, 30, 180)},
        
        {EndpointBucketName.UpdateUser, new(240, 20, 180)},
        {EndpointBucketName.HeartUser, new(300, 30, 180)},
        {EndpointBucketName.UploadFriendData, new(240, 6, 180)},
        {EndpointBucketName.DeleteOwnUser, new(600, 6, 480)},
#endregion
        
#region Moderation
        {EndpointBucketName.UploadGriefReport, new(400, 10, 240)},
        {EndpointBucketName.FilterModeratedAssets, new(300, 60, 180)},
        // all because of adventure uploading
        {EndpointBucketName.FilterChatMessage, new(20, 900, 10)},
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
        
        {EndpointBucketName.ApiGetAssetInfo, new(240, 250, 120)},
#endregion
        
#region Matching
        {EndpointBucketName.GameUpdateRoomOrGetRooms, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {EndpointBucketName.ApiGetRooms, new(240, 90, 120)},
        {EndpointBucketName.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {EndpointBucketName.GetPlaylists, new(240, 40, 180)},
        {EndpointBucketName.GetLevelsFromPlaylist, new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general, so we need special-cases
        {EndpointBucketName.Lbp3GetPlaylists, new(240, 50, 180)},
        {EndpointBucketName.Lbp3GetLevelsFromPlaylist, new(240, 90, 180)},
        
        {EndpointBucketName.GetSinglePlaylist, new(240, 40, 180)},
        
        {EndpointBucketName.CreatePlaylist, new(240, 40, 180)},
        {EndpointBucketName.UpdatePlaylist, new(240, 50, 180)},
        {EndpointBucketName.HeartPlaylist, new(240, 30, 180)},
        {EndpointBucketName.DeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity
        {EndpointBucketName.GetActivityPage, new(240, 50, 180)},
#endregion

#region Notifications
        {EndpointBucketName.GetNotifications, new(240, 15, 180)},
        {EndpointBucketName.GetSingleNotification, new(240, 20, 180)},
        {EndpointBucketName.DeleteNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if fetching Genre categories fails
        {EndpointBucketName.GetCategories, new(240, 40, 180)},
#endregion
        
#region Instance
        {EndpointBucketName.GetGameConfig, new(240, 30, 180)},
        {EndpointBucketName.GetInstanceInfo, new(240, 20, 180)},
        {EndpointBucketName.GetInstanceStats, new(240, 15, 180)},
        {EndpointBucketName.GetEula, new(240, 15, 180)},
        {EndpointBucketName.GetAnnouncements, new(240, 15, 180)},
#endregion
        
#region Pins
        {EndpointBucketName.SyncPinProgress, new(240, 12, 180)},
#endregion
        
#region Challenges
        {EndpointBucketName.UploadPlayerChallenge, new(240, 8, 180)},
        {EndpointBucketName.UploadPlayerChallengeScore, new(240, 16, 180)},
        
        {EndpointBucketName.GetPlayerChallenges, new(240, 20, 180)},
        {EndpointBucketName.GetPlayerChallengeScores, new(240, 50, 180)},
        {EndpointBucketName.GetSinglePlayerChallengeScore, new(240, 40, 180)},
#endregion
        
#region Authentication
    {EndpointBucketName.GameLogin, new(300, 10, 300)},
    {EndpointBucketName.ApiLogin, new(300, 10, 300)},
    {EndpointBucketName.Register, new(3600, 10, 1800)},
    {EndpointBucketName.RefreshToken, new(300, 10, 300)},

    {EndpointBucketName.SendEmail, new(300, 10, 300)},
    {EndpointBucketName.VerifyEmailAddress, new(300, 10, 300)},
    {EndpointBucketName.ResetPassword, new(300, 10, 300)},
#endregion
    }.ToFrozenDictionary();
}