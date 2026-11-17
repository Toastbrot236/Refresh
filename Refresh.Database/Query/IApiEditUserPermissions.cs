namespace Refresh.Database.Query;

public interface IApiEditUserPermissions
{
    UserPermissionSetting? UploadAssetsPermission { get; set; }
    UserPermissionSetting? PublishNewLevelsPermission { get; set; }
    UserPermissionSetting? EditLevelsPermission { get; set; }
    UserPermissionSetting? SetLevelReuploadDataPermission { get; set; }
    UserPermissionSetting? UploadLevelScoresPermission { get; set; }
    UserPermissionSetting? HeartEntitiesPermission { get; set; }
    UserPermissionSetting? PublishPhotosPermission { get; set; }
    UserPermissionSetting? EditProfilePermission { get; set; }
    UserPermissionSetting? UploadCommentsPermission { get; set; }
    UserPermissionSetting? UploadReviewsPermission { get; set; }
    UserPermissionSetting? UploadPlaylistsPermission { get; set; }
    UserPermissionSetting? EditPlaylistsPermission { get; set; }
    UserPermissionSetting? UploadChallengesPermission { get; set; }
    UserPermissionSetting? UploadChallengeScoresPermission { get; set; }
}