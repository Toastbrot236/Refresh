namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

// Explanation on suffixes:
// Api = only used for API endpoints
// Game = only used for game endpoints (unless you're on a special-cased game)
// (specific game) = only used for game endpoints if you're on the specified game
// No prefix = shared across both game and API endpoints (by default)
//
// Usually, fetch endpoints have separated buckets, while post/patch/delete endpoints share buckets
public enum EndpointBucketName
{
    #region Misc
    Default,
    #endregion
        
    #region Levels
    GameGetListOfLevels,
    GameGetSingleLevel,
    ApiGetListOfLevels,
    ApiGetSingleLevel,
    ApiGetOwnRelationsToLevel,
    
    GamePrepareLevelPublish,
    GameRealLevelPublish,
    ApiEditLevel,
    
    DeleteLevel,
    HeartLevel,
    QueueLevel,
    TagLevel,
    RateLevel,
    ApiOverrideLevel,
    
    PspRateLevel,
    #endregion
        
    #region Level Scores
    GameGetListOfLevelScores,
    PspGetListOfLevelScores,
    
    ApiGetListOfLevelScores,
    ApiGetSingleLevelScore,
    
    GamePlayLevel,
    PspPlayLevel,
    
    GameUploadLevelScore,
    PspUploadLevelScore,
    #endregion
        
    #region Reviews
    GameGetListOfReviews,
    GameGetSingleReview,
    ApiGetListOfReviews,
    ApiGetSingleReview,
    
    UploadReview,
    RateReview,
    DeleteReview,
    #endregion
        
    #region Comments (both Profile and Level)
    GameGetListOfComments, 
    GameGetSingleComment,
    ApiGetListOfComments,
    ApiGetSingleComment,
    
    UploadComment,
    RateComment,
    DeleteComment,
    #endregion
        
    #region Photos
    GameGetListOfPhotos,
    GameGetSinglePhoto,
    ApiGetListOfPhotos,
    ApiGetSinglePhoto,
    
    GameUploadPhoto,
    DeletePhoto,
    #endregion
        
    #region Users
    GameGetListOfUsers,
    GameGetSingleUser,
    ApiGetListOfUsers,
    ApiGetSingleUser,
    ApiGetOwnUser,
    
    UpdateUser,
    GameUploadFriendData,
    GameSyncUserPrivacySettings,
    HeartUser,
    ApiDeleteOwnUser,
    #endregion
        
    #region Moderation
    GameUploadGriefReport,
    GameFilterModeratedAssets,
    GameFilterChatMessage,
    #endregion
        
    #region Assets
    GameUploadAsset,
    GameDownloadAsset,
    
    ApiDownloadAsset,
    ApiDownloadImage,
    ApiGetAssetMetadata,
    ApiUploadImage,
    #endregion
        
    #region Matching
    GameUpdateRoomOrGetRooms,
    ApiGetListOfRooms,
    ApiGetSingleRoom,
    #endregion
        
    #region Playlists
    GameGetListOfPlaylists,
    GameGetPlaylistContents,
    
    Lbp3GetListOfPlaylists,
    Lbp3GetPlaylistContents,
    
    ApiGetListOfPlaylists,
    ApiGetSinglePlaylist,
    
    CreatePlaylist,
    UpdatePlaylistMetadata,
    UpdatePlaylistContents,
    HeartPlaylist,
    DeletePlaylist,
    #endregion
        
    #region Activity
    GameGetActivityPage,
    ApiGetActivityPage,
    #endregion
    
    #region Notifications
    GameGetListOfNotifications,
    ApiGetListOfNotifications,
    ApiGetSingleNotification,
    ApiDeleteNotification,
    #endregion
        
    #region Categories
    GameGetListOfCategories,
    ApiGetListOfCategories,
    #endregion
        
    #region Contests
    ApiGetListOfContests,
    ApiGetSingleContest,
    #endregion
        
    #region Instance
    GameGetGameConfig,
    GameGetInstanceStats,
    ApiGetInstanceStats,
    ApiGetInstanceInfo,
    ApiGetDocumentation,
    
    GameGetEula,
    GameGetListOfAnnouncements,
    ApiGetListOfAnnouncements,
    #endregion
        
    #region Pins
    GameSyncPinProgress,
    #endregion
        
    #region Challenges
    GameGetListOfPlayerChallenges,
    GameGetListOfPlayerChallengeScores,
    GameGetSinglePlayerChallengeScore,
    
    GameUploadPlayerChallenge,
    GameUploadPlayerChallengeScore,
    #endregion

    #region Authentication
    GameLogin,
    ApiLogin,
    ApiRegister,
    ApiRequestEmail,
    ApiVerifyEmailAddress,
    ApiResetPassword,
    ApiGetListOfIpAddresses,
    ApiApproveOrDenyIpAddress,
    #endregion
}