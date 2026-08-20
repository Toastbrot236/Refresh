namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

/*
 * TODO should more fetch endpoints have their buckets split between game and API?
 * Considering that it might not be great UX if you e.g. request too many lists on API and then can no longer
 * get any in-game either, and because we usually do more DB calls when returning certain entities over API vs in-game.
 */
public enum GameEndpointBucketName
{
    #region Misc
    Default,
    #endregion
        
    #region Levels
    GetListOfLevels,
    GetSingleLevel,
    
    PrepareLevelPublish,
    RealLevelPublish,
    
    DeleteLevel,
    HeartLevel,
    QueueLevel,
    TagLevel,
    RateLevel,
    
    PspRateLevel,
    #endregion
        
    #region Level Scores
    GetListOfLevelScores,
    PspGetListOfLevelScores,
    
    PlayLevel,
    PspPlayLevel,
    
    UploadLevelScore,
    PspUploadLevelScore,
    #endregion
        
    #region Reviews
    GetListOfReviews,
    GetSingleReview,
    
    UploadReview,
    RateReview,
    DeleteReview,
    #endregion
        
    #region Comments (both Profile and Level)
    GetListOfComments, 
    
    UploadComment,
    RateComment,
    DeleteComment,
    #endregion
        
    #region Photos
    GetListOfPhotos,
    GetSinglePhoto,
    
    UploadPhoto,
    DeletePhoto,
    #endregion
        
    #region Users
    GetListOfUsers,
    GetSingleUser,
    
    UpdateUser,
    UploadFriendData,
    SyncUserPrivacySettings,
    HeartUser,
    #endregion
        
    #region Moderation
    UploadGriefReport,
    FilterModeratedAssets,
    FilterChatMessage,
    #endregion
        
    #region Assets
    UploadAsset,
    DownloadAsset,
    #endregion
        
    #region Matching
    UpdateRoomOrGetRooms,
    #endregion
        
    #region Playlists
    GetListOfPlaylists,
    GetPlaylistContents,
    
    Lbp3GetListOfPlaylists,
    Lbp3GetLevelsFromPlaylist,
    
    CreatePlaylist,
    UpdatePlaylistMetadata,
    UpdatePlaylistContents,
    HeartPlaylist,
    DeletePlaylist,
    #endregion
        
    #region Activity
    GetActivityPage,
    #endregion
    
    #region Notifications
    GetListOfNotifications,
    #endregion
        
    #region Categories
    GetListOfCategories,
    #endregion
        
    #region Instance
    GetGameConfig,
    GetInstanceStats,
    GetEula,
    GetListOfAnnouncements,
    #endregion
        
    #region Pins
    SyncPinProgress,
    #endregion
        
    #region Challenges
    GetListOfPlayerChallenges,
    GetListOfPlayerChallengeScores,
    GetSinglePlayerChallengeScore,
    
    UploadPlayerChallenge,
    UploadPlayerChallengeScore,
    #endregion

    #region Authentication
    Login,
    #endregion
}