namespace Refresh.Core.RateLimits;

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
    GetUsers,
    GetUsersByListOfNames,
    GetSingleUser,
    ApiGetOwnUser,
    
    UpdateUser,
    UploadFriendData,
    HeartUser,
    
    DeleteOwnUser,
    #endregion
        
    #region Moderation
    UploadGriefReport,
    FilterModeratedAssets,
    FilterChatMessage,
    #endregion
        
    #region Assets
    GameUploadAsset,
    GameDownloadAsset,
    
    ApiUploadImage,
    ApiDownloadAsset,
    ApiDownloadImage,
    
    ApiGetAssetInfo,
    #endregion
        
    #region Matching
    GameUpdateRoomOrGetRooms,
    
    ApiGetRooms,
    ApiGetSingleRoom,
    #endregion
        
    #region Playlists
    GetPlaylists,
    GetLevelsFromPlaylist,
    GetSinglePlaylist,
    
    Lbp3GetPlaylists,
    Lbp3GetLevelsFromPlaylist,
    
    CreatePlaylist,
    UpdatePlaylist,
    HeartPlaylist,
    DeletePlaylist,
    #endregion
        
    #region Activity
    GetActivityPage,
    #endregion
    
    #region Notifications
    GetNotifications,
    GetSingleNotification,
    DeleteNotification,
    #endregion
        
    #region Categories
    GetCategories,
    #endregion
        
    #region Instance
    GetGameConfig,
    GetInstanceInfo,
    GetInstanceStats,
    GetEula,
    GetAnnouncements,
    #endregion
        
    #region Pins
    SyncPinProgress,
    #endregion
        
    #region Challenges
    GetPlayerChallenges,
    GetPlayerChallengeScores,
    GetSinglePlayerChallengeScore,
    
    UploadPlayerChallenge,
    UploadPlayerChallengeScore,
    #endregion

    #region Authentication
    GameLogin,
    ApiLogin,
    Register,
    RefreshToken,
    
    SendEmail,
    VerifyEmailAddress,
    ResetPassword,
    
    #endregion
}