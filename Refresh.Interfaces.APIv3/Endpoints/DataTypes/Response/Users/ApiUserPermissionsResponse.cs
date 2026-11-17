using Refresh.Core.Types.Data;
using Refresh.Database.Models.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUserPermissionsResponse : IApiResponse, IDataConvertableFrom<ApiUserRestrictionsResponse, GameUser>
{
    public UserPermissionSetting UploadAssetsPermission { get; set; }
    public UserPermissionSetting PublishNewLevelsPermission { get; set; }
    public UserPermissionSetting EditLevelsPermission { get; set; }
    public UserPermissionSetting SetLevelReuploadDataPermission { get; set; }
    public UserPermissionSetting UploadLevelScoresPermission { get; set; }
    public UserPermissionSetting HeartEntitiesPermission { get; set; }
    public UserPermissionSetting PublishPhotosPermission { get; set; }
    public UserPermissionSetting EditProfilePermission { get; set; }
    public UserPermissionSetting UploadCommentsPermission { get; set; }
    public UserPermissionSetting UploadReviewsPermission { get; set; }
    public UserPermissionSetting UploadPlaylistsPermission { get; set; }
    public UserPermissionSetting EditPlaylistsPermission { get; set; }
    public UserPermissionSetting UploadChallengesPermission { get; set; }
    public UserPermissionSetting UploadChallengeScoresPermission { get; set; }
    
    public static ApiUserPermissionsResponse? FromOld(GameUser? user, DataContext dataContext)
    {
        if (user == null) return null;
        
        return new ApiUserPermissionsResponse
        {
            UploadAssetsPermission = user.UploadAssetsPermission,
            PublishNewLevelsPermission = user.PublishNewLevelsPermission,
            EditLevelsPermission = user.EditLevelsPermission,
            SetLevelReuploadDataPermission = user.SetLevelReuploadDataPermission,
            UploadLevelScoresPermission = user.UploadLevelScoresPermission,
            HeartEntitiesPermission = user.HeartEntitiesPermission,
            PublishPhotosPermission = user.PublishPhotosPermission,
            EditProfilePermission = user.EditProfilePermission,
            UploadCommentsPermission = user.UploadCommentsPermission,
            UploadReviewsPermission = user.UploadReviewsPermission,
            UploadPlaylistsPermission = user.UploadPlaylistsPermission,
            EditPlaylistsPermission = user.EditPlaylistsPermission,
            UploadChallengesPermission = user.UploadChallengesPermission,
            UploadChallengeScoresPermission = user.UploadChallengeScoresPermission,
        };
    }

    public static IEnumerable<ApiUserRestrictionsResponse> FromOldList(IEnumerable<GameUser> oldList, DataContext dataContext)
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}