using Refresh.Database.Query;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Moderation;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUpdateUserPermissionsRequest : ApiModerationRequest, IApiEditUserPermissions
{
    public UserPermissionSetting? UploadAssetsPermission { get; set; }
    public UserPermissionSetting? PublishNewLevelsPermission { get; set; }
    public UserPermissionSetting? EditLevelsPermission { get; set; }
    public UserPermissionSetting? SetLevelReuploadDataPermission { get; set; }
    public UserPermissionSetting? UploadLevelScoresPermission { get; set; }
    public UserPermissionSetting? HeartEntitiesPermission { get; set; }
    public UserPermissionSetting? PublishPhotosPermission { get; set; }
    public UserPermissionSetting? EditProfilePermission { get; set; }
    public UserPermissionSetting? UploadCommentsPermission { get; set; }
    public UserPermissionSetting? UploadReviewsPermission { get; set; }
    public UserPermissionSetting? UploadPlaylistsPermission { get; set; }
    public UserPermissionSetting? EditPlaylistsPermission { get; set; }
    public UserPermissionSetting? UploadChallengesPermission { get; set; }
    public UserPermissionSetting? UploadChallengeScoresPermission { get; set; }
}