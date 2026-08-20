using System.Collections.Frozen;
using Refresh.Core.Configuration;

namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

public static class ApiEndpointBucketDefaults
{
    public static readonly FrozenDictionary<ApiEndpointBucketName, ConfigRateLimitBucket> Defaults = new Dictionary<ApiEndpointBucketName, ConfigRateLimitBucket>()
    {
#region Misc
        {ApiEndpointBucketName.Default, new(90, 380, 45)},
#endregion
        
#region Levels
        {ApiEndpointBucketName.GetListOfLevels, new(240, 50, 180)},
        {ApiEndpointBucketName.GetSingleLevel, new(240, 50, 180)},
        {ApiEndpointBucketName.GetOwnRelationsOnLevel, new(240, 50, 180)},
        
        {ApiEndpointBucketName.EditLevel, new(300, 20, 180)},
        {ApiEndpointBucketName.DeleteLevel, new(300, 20, 180)},
        {ApiEndpointBucketName.HeartLevel, new(300, 30, 180)},
        {ApiEndpointBucketName.QueueLevel, new(300, 50, 180)},
        {ApiEndpointBucketName.TagLevel, new(300, 10, 180)},
        {ApiEndpointBucketName.RateLevel, new(300, 20, 180)},
        
        {ApiEndpointBucketName.PspRateLevel, new(420, 90, 240)},
#endregion
        
#region Level Scores
        {ApiEndpointBucketName.PlayLevel, new(300, 40, 180)},
        {ApiEndpointBucketName.UploadLevelScore, new(300, 30, 180)},
        {ApiEndpointBucketName.GetLevelScores, new(300, 70, 180)},
        
        {ApiEndpointBucketName.PspPlayLevel, new(300, 30, 180)},
        {ApiEndpointBucketName.PspUploadLevelScore, new(300, 30, 180)},
        // PSP spams these requests for story levels every time the download moon is loaded, apparently.
        {ApiEndpointBucketName.PspGetLevelScores, new(300, 150, 180)},
#endregion
        
#region Reviews
        {ApiEndpointBucketName.GetReviews, new(300, 60, 180)},
        {ApiEndpointBucketName.GetSingleReview, new(300, 30, 180)},
        
        {ApiEndpointBucketName.UploadReview, new(300, 12, 180)},
        {ApiEndpointBucketName.RateReview, new(300, 40, 180)},
        {ApiEndpointBucketName.DeleteReview, new(300, 30, 180)},
#endregion
        
#region Comments (both Profile and Level)
        {ApiEndpointBucketName.GetComments, new(300, 60, 180)},
        
        {ApiEndpointBucketName.UploadComment, new(300, 18, 180)},
        {ApiEndpointBucketName.RateComment, new(300, 40, 180)},
        {ApiEndpointBucketName.DeleteComment, new(300, 30, 180)},
#endregion
        
#region Photos
        {ApiEndpointBucketName.GetPhotos, new(300, 60, 180)},
        {ApiEndpointBucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {ApiEndpointBucketName.GetPhotos, new(300, 60, 180)},
        {ApiEndpointBucketName.GetSinglePhoto, new(300, 30, 180)},
        
        {ApiEndpointBucketName.UploadPhoto, new(300, 25, 180)},
        {ApiEndpointBucketName.DeletePhoto, new(300, 30, 180)},
#endregion
        
#region Users
        // LBP3 sometimes sends useless data here in the background
        
        {ApiEndpointBucketName.GetUsers, new(240, 60, 180)},
        {ApiEndpointBucketName.GetSingleUser, new(240, 60, 180)},
        {ApiEndpointBucketName.GetUsersByListOfNames, new(240, 30, 180)},
        
        {ApiEndpointBucketName.UpdateUser, new(240, 20, 180)},
        {ApiEndpointBucketName.HeartUser, new(300, 30, 180)},
        {ApiEndpointBucketName.UploadFriendData, new(240, 6, 180)},
        {ApiEndpointBucketName.DeleteOwnUser, new(600, 6, 480)},
#endregion
        
#region Moderation
        {ApiEndpointBucketName.UploadGriefReport, new(400, 10, 240)},
        {ApiEndpointBucketName.FilterModeratedAssets, new(300, 60, 180)},
        // all because of adventure uploading
        {ApiEndpointBucketName.FilterChatMessage, new(20, 900, 10)},
#endregion
        
#region Assets
        // Regular download limits are this high on both game and API because both the game and third party API clients
        // (e.g. archive_dl) are likely to download many of these at times depending on what level they're trying to load
        // (additionally, adventures can have even more dependencies!)
        {ApiEndpointBucketName.GameUploadAsset, new(300, 150, 180)},
        {ApiEndpointBucketName.GameDownloadAsset, new(240, 500, 120)},
        
        {ApiEndpointBucketName.ApiUploadImage, new(300, 20, 180)},
        {ApiEndpointBucketName.ApiDownloadAsset, new(240, 500, 120)},
        {ApiEndpointBucketName.ApiDownloadImage, new(240, 250, 120)},
        
        {ApiEndpointBucketName.ApiGetAssetInfo, new(240, 250, 120)},
#endregion
        
#region Matching
        {ApiEndpointBucketName.GameUpdateRoomOrGetRooms, new(240, 30, 120)},
        
        // this high because of beta website's fake live updating
        {ApiEndpointBucketName.ApiGetRooms, new(240, 90, 120)},
        {ApiEndpointBucketName.ApiGetSingleRoom, new(240, 40, 120)},
#endregion
        
#region Playlists
        {ApiEndpointBucketName.GetPlaylists, new(240, 40, 180)},
        {ApiEndpointBucketName.GetLevelsFromPlaylist, new(240, 50, 180)},
        // LBP3 doesn't cache these at all, and is inefficient with them in general, so we need special-cases
        {ApiEndpointBucketName.Lbp3GetPlaylists, new(240, 50, 180)},
        {ApiEndpointBucketName.Lbp3GetLevelsFromPlaylist, new(240, 90, 180)},
        
        {ApiEndpointBucketName.GetSinglePlaylist, new(240, 40, 180)},
        
        {ApiEndpointBucketName.CreatePlaylist, new(240, 40, 180)},
        {ApiEndpointBucketName.UpdatePlaylist, new(240, 50, 180)},
        {ApiEndpointBucketName.HeartPlaylist, new(240, 30, 180)},
        {ApiEndpointBucketName.DeletePlaylist, new(240, 30, 180)},
#endregion
        
#region Activity
        {ApiEndpointBucketName.GetActivityPage, new(240, 50, 180)},
#endregion

#region Notifications
        {ApiEndpointBucketName.GetNotifications, new(240, 15, 180)},
        {ApiEndpointBucketName.GetSingleNotification, new(240, 20, 180)},
        {ApiEndpointBucketName.DeleteNotification, new(240, 20, 180)},
#endregion
        
#region Categories
        // LBP3 spams if fetching Genre categories fails
        {ApiEndpointBucketName.GetCategories, new(240, 40, 180)},
#endregion
        
#region Instance
        {ApiEndpointBucketName.GetGameConfig, new(240, 30, 180)},
        {ApiEndpointBucketName.GetInstanceInfo, new(240, 20, 180)},
        {ApiEndpointBucketName.GetInstanceStats, new(240, 15, 180)},
        {ApiEndpointBucketName.GetEula, new(240, 15, 180)},
        {ApiEndpointBucketName.GetAnnouncements, new(240, 15, 180)},
#endregion
        
#region Pins
        {ApiEndpointBucketName.SyncPinProgress, new(240, 12, 180)},
#endregion
        
#region Challenges
        {ApiEndpointBucketName.UploadPlayerChallenge, new(240, 8, 180)},
        {ApiEndpointBucketName.UploadPlayerChallengeScore, new(240, 16, 180)},
        
        {ApiEndpointBucketName.GetPlayerChallenges, new(240, 20, 180)},
        {ApiEndpointBucketName.GetPlayerChallengeScores, new(240, 50, 180)},
        {ApiEndpointBucketName.GetSinglePlayerChallengeScore, new(240, 40, 180)},
#endregion
        
#region Authentication
    {ApiEndpointBucketName.GameLogin, new(300, 10, 300)},
    {ApiEndpointBucketName.ApiLogin, new(300, 10, 300)},
    {ApiEndpointBucketName.Register, new(3600, 10, 1800)},
    {ApiEndpointBucketName.RefreshToken, new(300, 10, 300)},

    {ApiEndpointBucketName.SendEmail, new(300, 10, 300)},
    {ApiEndpointBucketName.VerifyEmailAddress, new(300, 10, 300)},
    {ApiEndpointBucketName.ResetPassword, new(300, 10, 300)},
#endregion
    }.ToFrozenDictionary();
}