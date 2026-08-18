namespace Refresh.Core.RateLimits;

/*
 * TODO should more fetch endpoints have their buckets split between game and API?
 * Considering that it might not be great UX if you e.g. request too many lists on API and then can no longer
 * get any in-game either, and because we usually do more DB calls when returning certain entities over API vs in-game.
 */
public enum ApiEndpointBucketName
{
    #region Misc
    Default,
    #endregion
        
    #region Levels
    GetListOfLevels,
    GetSingleLevel,
    
    EditLevel,
    DeleteLevel,
    
    HeartLevel,
    QueueLevel,
    TagLevel,
    RateLevel,
    #endregion
        
    #region Level Scores
    GetListOfLevelScores,
    GetSingleLevelScore,
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
    GetSingleComment,
    
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
    GetOwnUser,
    
    UpdateOwnUser,
    HeartUser,
    DeleteOwnUser,
    #endregion
        
    #region Assets
    UploadImage,
    DownloadRawAsset,
    DownloadImage,
    
    GetAssetInfo,
    #endregion
        
    #region Matching
    GetListOfRooms,
    GetSingleRoom,
    #endregion
        
    #region Playlists
    GetListOfPlaylists,
    GetSinglePlaylist,
    
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
    GetLevelCategories,
    GetUserCategories,
    #endregion
        
    #region Instance
    GetInstanceInfo,
    GetInstanceStats,
    GetEula,
    GetAnnouncements,
    #endregion

    #region Authentication
    Login,
    Register,
    RefreshToken,
    
    SendEmail,
    VerifyEmailAddress,
    ResetPassword,
    
    GetListOfIpAddresses,
    ApproveOrDenyIpAddress,
    
    #endregion
}