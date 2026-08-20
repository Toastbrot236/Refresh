namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

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
    GetOwnRelationsOnLevel,
    
    EditLevel,
    DeleteLevel,
    
    HeartLevel,
    QueueLevel,
    TagLevel,
    RateLevel,
    OverrideLevel,
    #endregion
        
    #region Level Scores
    GetListOfLevelScores,
    GetSingleLevelScore,
    #endregion
        
    #region Reviews
    GetListOfReviews,
    GetSingleReview,
    
    UploadReview,
    UpdateReview,
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
    
    UpdateUser,
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
    GetSingleNotification,
    DeleteNotification,
    #endregion
        
    #region Categories
    GetListOfCategories,
    #endregion
        
    #region Contests
    GetListOfContests,
    GetSingleContest,
    #endregion
        
    #region Instance
    GetInstanceInfo,
    GetInstanceStats,
    GetApiDocumentation,
    GetEula,
    GetListOfAnnouncements,
    #endregion

    #region Authentication
    Login,
    Register,
    
    RequestEmail,
    VerifyEmailAddress,
    ResetPassword,
    
    GetListOfIpAddresses,
    ApproveOrDenyIpAddress,
    
    #endregion
}