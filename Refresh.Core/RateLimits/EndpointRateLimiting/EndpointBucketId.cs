namespace Refresh.Core.RateLimits.EndpointRateLimiting;

// TODO add IDs for all API buckets here
// TODO separate buckets for PSP for certain endpoints, since the ones in question are spammed by PSP in certain cases.
// Generally, fetch endpoints should use separate buckets depending on whether they are game/API endpoints,
// while upload/modification/deletion endpoints should share buckets.
public enum EndpointBucketId
{
    #region Misc
    Default,
    #endregion

    #region Authentication
    GameLogin,
    #endregion

    #region Instance
    GameGetGameConfig,
    GameGetInstanceStats,

    GameGetEula,
    GameGetListOfAnnouncements,
    #endregion

    #region Categories
    GameGetListOfCategories,
    #endregion

    #region Levels
    GameGetListOfLevels,
    GameGetSingleLevel,
    ApiGetSingleLevel,

    GamePrepareLevelPublish,
    GameRealLevelPublish,
    ApiEditLevel,

    DeleteLevel,
    HeartLevel,
    QueueLevel,
    TagLevel,
    RateLevel,
    #endregion

    #region Level Scores
    GameGetListOfLevelScores,
    GameUploadLevelScore,
    
    GamePlayLevel,
    #endregion

    #region Reviews
    GameGetListOfReviews,
    GameGetSingleReview,

    UploadReview,
    RateReview,
    DeleteReview,
    #endregion

    #region Comments (both Profile and Level)
    GameGetListOfComments, 
    GameGetSingleComment,

    UploadComment,
    RateComment,
    DeleteComment,
    #endregion

    #region Photos
    GameGetListOfPhotos,
    GameGetSinglePhoto,

    GameUploadPhoto,
    DeletePhoto,
    #endregion

    #region Users
    GameGetListOfUsers,
    GameGetSingleUser,

    UpdateUser,
    GameUploadFriendData,
    GameSyncUserPrivacySettings,
    HeartUser,
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
    #endregion

    #region Playlists
    GameGetListOfPlaylists,
    GameGetPlaylistContents,

    Lbp3GetListOfPlaylists,
    Lbp3GetPlaylistContents,

    CreatePlaylist,
    UpdatePlaylistMetadata,
    UpdatePlaylistContents,
    HeartPlaylist,
    DeletePlaylist,
    #endregion

    #region Activity
    GameGetActivityPage,
    #endregion

    #region Notifications
    GameGetListOfNotifications,
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
}