namespace Refresh.Core.RateLimits;

// Check out the comments in BucketDefaults on naming,
// why some buckets are split while others are shared, etc.
public enum BucketName
{
    #region Misc
    Unknown,
    Global,
    #endregion
        
    #region Levels
    GameStartPublishLevel,
    GameFullyPublishLevel,
    
    GameGetLevelsFromCategory,
    GameGetSingleLevel,
    GameGetLevelsByIds,
    
    ApiEditLevel,
    ApiGetLevelsFromCategory,
    ApiGetSingleLevel,
    
    AnyDeleteLevel,
    AnyHeartLevel,
    AnyQueueLevel,
    AnyTagLevel,
    AnyRateLevel,
    PspRateLevel,
    #endregion
        
    #region Level Scores
    GamePlayLevel,
    GameSubmitLevelScore,
    GameGetLevelScores,
    
    PspPlayLevel,
    PspSubmitLevelScore,
    PspGetLevelScores,
    
    ApiGetLevelScores,
    #endregion
        
    #region Reviews
    GameGetReviews,
    ApiGetReviews,
    ApiGetSingleReview,
    
    AnySubmitReview,
    AnyRateReview,
    AnyDeleteReview,
    #endregion
        
    #region Comments (both Profile and Level)
    GameGetComments, 
    ApiGetComments,
    
    AnySubmitComment,
    AnyRateComment,
    AnyDeleteComment,
    #endregion
        
    #region Photos
    GameUploadPhoto,
    GameGetPhotos,
    GameGetSinglePhoto,
    
    ApiGetPhotos,
    ApiGetSinglePhoto,
    AnyDeletePhoto,
    #endregion
        
    #region Users
    GameUpdateUser,
    GameUpdateWebsitePrivacy,
    GameUploadNpUserData,
    
    GameGetUsersFromCategory,
    GameGetSingleUser,
    GameGetUsersByNames,
    
    ApiUpdateUser,
    ApiDeleteOwnUser,
    
    ApiGetUsersFromCategory,
    ApiGetSingleUser,
    AnyHeartUser,
    #endregion
        
    #region Moderation
    GameUploadGriefReport,
    GameFilterModeratedAssets,
    GameFilterMessage,
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
    GameRoomRequest,
    ApiGetRooms,
    ApiGetSingleRoom,
    #endregion
        
    #region Playlists
    Lbp1GetPlaylists,
    Lbp1GetSlotsFromPlaylist,
    
    Lbp3GetLevelsFromPlaylist,
    Lbp3GetPlaylists,
    
    ApiGetPlaylistsFromCategory,
    ApiGetSinglePlaylist,
    
    AnyCreatePlaylist,
    AnyUpdatePlaylist,
    AnyHeartPlaylist,
    AnyDeletePlaylist,
    #endregion
        
    #region Activity + Notifications
    GameGetActivity,
    GameGetNotifications,
    
    ApiGetActivity,
    ApiGetNotifications,
    ApiGetSingleNotification,
    #endregion
        
    #region Categories
    GameGetCategories,
    GameGetLevelCategories,
    GameGetUserCategories,
    #endregion
        
    #region Instance
    GameGetConfig,
    GameGetInstanceStats,
    GameGetEula,
    GameGetAnnouncements,
    
    ApiGetInstanceInfo,
    ApiGetInstanceStats,
    ApiGetAnnouncements,
    #endregion
        
    #region Pins
    GameSyncPins,
    #endregion
        
    #region Challenges
    GameUploadChallenge,
    GameUploadChallengeScore,
    
    GameGetChallenges,
    GameGetChallengeScores,
    GameGetSingleChallengeScore,
    #endregion
}