namespace Refresh.Core.RateLimits.EndpointRateLimiting.Buckets;

// Explanation on suffixes:
// Api = only used for API endpoints
// Game = only used for game endpoints (unless you're on a special-cased game)
// (specific game) = only used for game endpoints if you're on the specified game
// Shared = shared across both game and API endpoints
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
    
    SharedDeleteLevel,
    SharedHeartLevel,
    SharedQueueLevel,
    SharedTagLevel,
    SharedRateLevel,
    ApiOverrideLevel,
    
    PspRateLevel,
    #endregion
        
    #region Level Scores
    GameGetListOfLevelScores,
    GamePspGetListOfLevelScores,
    ApiGetListOfLevelScores,
    ApiPspGetListOfLevelScores,
    
    GamePlayLevel,
    GamePspPlayLevel,
    
    GameUploadLevelScore,
    GamePspUploadLevelScore,
    #endregion
        
    #region Reviews
    GameGetListOfReviews,
    GameGetSingleReview,
    ApiGetListOfReviews,
    ApiGetSingleReview,
    
    SharedUploadReview,
    SharedRateReview,
    SharedDeleteReview,
    #endregion
        
    #region Comments (both Profile and Level)
    GameGetListOfComments, 
    GameGetSingleComment,
    ApiGetListOfComments,
    ApiGetSingleComment,
    
    SharedUploadComment,
    SharedRateComment,
    SharedDeleteComment,
    #endregion
        
    #region Photos
    GameGetListOfPhotos,
    GameGetSinglePhoto,
    ApiGetListOfPhotos,
    ApiGetSinglePhoto,
    
    GameUploadPhoto,
    SharedDeletePhoto,
    #endregion
        
    #region Users
    GameGetListOfUsers,
    GameGetSingleUser,
    ApiGetListOfUsers,
    ApiGetSingleUser,
    ApiGetOwnUser,
    
    SharedUpdateUser,
    GameUploadFriendData,
    GameSyncUserPrivacySettings,
    SharedHeartUser,
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
    
    SharedCreatePlaylist,
    SharedUpdatePlaylistMetadata,
    SharedUpdatePlaylistContents,
    SharedHeartPlaylist,
    SharedDeletePlaylist,
    #endregion
        
    #region Activity
    GameGetActivityPage,
    ApiGetActivityPage,
    #endregion
    
    #region Notifications
    GameGetListOfNotifications,
    ApiGetListOfNotifications,
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