namespace Refresh.Core.RateLimits;

public static class BucketNames
{
    #region Misc
    public const string Global = "global";
    #endregion
        
    #region Levels
    public const string GameStartPublishLevel = "gameStartPublishLevel";
    public const string GameFullyPublishLevel = "gameFullyPublishLevel";
    public const string GameGetCategoryLevelList = "gameGetCategoryLevelList";
    public const string GameGetSingleLevel = "gameGetSingleLevel";
    public const string GameGetLevelsByIds = "gameGetLevelsByIds";
    public const string ApiEditLevel = "apiEditLevel";
    public const string ApiGetLevelsFromCategory = "apiGetLevelsFromCategory";
    public const string ApiGetSingleLevel = "apiGetSingleLevel";
    public const string AnyDeleteLevel = "anyDeleteLevel";
    public const string AnyHeartLevel = "anyHeartLevel";
    public const string AnyQueueLevel = "anyQueueLevel";
    public const string AnyTagLevel = "anyTagLevel";
    public const string AnyRateLevel = "anyRateLevel";
    public const string PspRateLevel = "pspRateLevel";
    #endregion
        
    #region Level Scores
    public const string GamePlayLevel = "gamePlayLevel";
    public const string GameSubmitLevelScore = "gameSubmitLevelScore";
    public const string GameGetLevelScores = "gameGetLevelScores";
    public const string PspPlayLevel = "pspPlayLevel";
    public const string PspSubmitLevelScore = "pspSubmitLevelScore";
    public const string PspGetLevelScores = "pspGetLevelScores";
    public const string ApiGetLevelScores = "apiGetLevelScores";
    #endregion
        
    #region Reviews
    public const string GameGetReviews = "gameGetReviews";
    public const string ApiGetReviews = "apiGetReviews";
    public const string AnySubmitReview = "anySubmitReview";
    public const string AnyRateReviews = "anyRateReviews";
    public const string AnyDeleteReviews = "anyDeleteReviews";
    #endregion
        
    #region Comments (both Profile and Level)
    public const string GameGetComments = "gameGetComments";
    public const string ApiGetComments = "apiGetComments";
    public const string AnySubmitComment = "anySubmitComment";
    public const string AnyRateComments = "anyRateComments";
    public const string AnyDeleteComments = "anyDeleteComments";
    #endregion
        
    #region Photos
    public const string GameUploadPhoto = "gameUploadPhoto";
    public const string GameGetPhotos = "gameGetPhotos";
    public const string GameGetSinglePhoto = "gameGetSinglePhoto";
    public const string ApiGetPhotos = "apiGetPhotos";
    public const string ApiGetSinglePhoto = "apiGetSinglePhoto";
    public const string AnyDeletePhotos = "anyDeletePhotos";
    #endregion
        
    #region Users
    public const string GameUpdateUser = "gameUpdateUser";
    public const string GameUpdateWebsitePrivacy = "gameUpdateWebsitePrivacy";
    public const string GameGetUsersFromCategory = "gameGetUsersFromCategory";
    public const string GameGetSingleUser = "gameGetSingleUser";
    public const string GameGetUsersByNames = "gameGetUsersByNames";
    public const string GameUploadNpUserData = "gameUploadNpUserData";
    public const string ApiUpdateUser = "apiUpdateUser";
    public const string ApiDeleteOwnUser = "apiDeleteOwnUser";
    public const string ApiGetUsersFromCategory = "apiGetUsersFromCategory";
    public const string ApiGetSingleUser = "apiGetSingleUser";
    public const string AnyHeartUser = "anyHeartUser";
    #endregion
        
    #region Moderation
    public const string GameUploadGriefReport = "gameUploadGriefReport";
    public const string GameFilterModeratedAssetList = "gameFilterModeratedAssetList";
    public const string GameFilterMessage = "gameFilterMessage";
    #endregion
        
    #region Assets
    public const string GameUploadAsset = "gameUploadAsset";
    public const string GameDownloadAsset = "gameDownloadAsset";
    public const string ApiUploadImage = "apiUploadImage";
    public const string ApiDownloadAsset = "apiDownloadAsset";
    public const string ApiDownloadImage = "apiDownloadImage";
    #endregion
        
    #region Matching
    public const string GameRoomRequest = "gameRoomRequest";
    public const string ApiGetRooms = "apiGetRooms";
    public const string ApiGetSingleRoom = "apiGetSingleRoom";
    #endregion
        
    #region Playlists
    public const string Lbp1GetPlaylists = "lbp1GetPlaylists";
    public const string Lbp1GetSlotsFromPlaylist = "lbp1GetSlotsFromPlaylist";
    public const string Lbp3GetLevelsFromPlaylist = "lbp3GetLevelsFromPlaylist";
    public const string Lbp3GetPlaylistsByUser = "lbp3GetPlaylistsByUser";
    public const string ApiGetPlaylistsFromCategory = "apiGetPlaylistsFromCategory";
    public const string ApiGetSinglePlaylist = "apiGetSinglePlaylist";
    public const string AnyCreatePlaylist = "anyCreatePlaylist";
    public const string AnyUpdatePlaylist = "anyUpdatePlaylist";
    public const string AnyHeartPlaylist = "anyHeartPlaylist";
    public const string AnyDeletePlaylist = "anyDeletePlaylist";
    #endregion
        
    #region Activity + Notifications
    public const string GameGetActivity = "gameGetActivity";
    public const string GameGetNotifications = "gameGetNotifications";
    public const string ApiGetActivity = "apiGetActivity";
    public const string ApiGetNotifications = "apiGetNotifications";
    public const string ApiGetSingleNotification = "apiGetSingleNotification";
    #endregion
        
    #region Categories
    public const string GameGetCategories = "gameGetCategories";
    public const string GameGetLevelCategories = "gameGetLevelCategories";
    public const string GameGetUserCategories = "gameGetUserCategories";
    #endregion
        
    #region Instance
    public const string GameGetConfig = "gameGetConfig";
    public const string GameGetInstanceStats = "gameGetInstanceStats";
    public const string GameGetEula = "gameGetEula";
    public const string GameGetAnnouncements = "gameGetAnnouncements";
    public const string ApiGetInstanceInfo = "apiGetInstanceInfo";
    public const string ApiGetInstanceStats = "apiGetInstanceStats";
    public const string ApiGetAnnouncements = "apiGetAnnouncements";
    #endregion
        
    #region Pins
    public const string GameSyncPins = "gameSyncPins";
    #endregion
        
    #region Challenges
    public const string GameUploadChallenge = "gameUploadChallenge";
    public const string GameUploadChallengeScore = "gameUploadChallengeScore";
    public const string GameGetChallenges = "gameGetChallenges";
    public const string GameGetChallengeScores = "gameGetChallengeScores";
    public const string GameGetSingleChallengeScore = "gameGetSingleChallengeScore";
    #endregion
}