using Refresh.Core.Types.Data;
using Refresh.Database.Models.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

/// <summary>
/// A user with full information, like current role, ban status, etc.
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUserPermissionsResponse : IApiResponse, IDataConvertableFrom<ApiUserPermissionsResponse, GameUser>
{
    public bool? IsAssetUploadingBlocked { get; set; }
    public bool? IsLevelPublishingBlocked { get; set; }
    public bool? IsLevelEditingBlocked { get; set; }
    public bool? IsLevelReattributingBlocked { get; set; }
    public bool? IsLevelScoreUploadingBlocked { get; set; }
    public bool? IsHeartingBlocked { get; set; }
    public bool? IsPhotoPublishingBlocked { get; set; }
    public bool? IsProfileUpdatingBlocked { get; set; }
    public bool? IsCommentPostingBlocked { get; set; }
    public bool? IsReviewPostingBlocked { get; set; }
    public bool? IsPlaylistUploadingBlocked { get; set; }
    public bool? IsPlaylistEditingBlocked { get; set; }
    public bool? IsChallengeUploadingBlocked { get; set; }
    public bool? IsChallengeScoreUploadingBlocked { get; set; }
    
    public static ApiUserPermissionsResponse? FromOld(GameUser? user, DataContext dataContext)
    {
        if (user == null) return null;
        
        return new ApiUserPermissionsResponse
        {
            IsAssetUploadingBlocked = user.IsAssetUploadingBlocked,
            IsLevelPublishingBlocked = user.IsLevelPublishingBlocked,
            IsLevelEditingBlocked = user.IsLevelEditingBlocked,
            IsLevelReattributingBlocked = user.IsLevelReattributingBlocked,
            IsLevelScoreUploadingBlocked = user.IsLevelScoreUploadingBlocked,
            IsHeartingBlocked = user.IsHeartingBlocked,
            IsPhotoPublishingBlocked = user.IsPhotoPublishingBlocked,
            IsProfileUpdatingBlocked = user.IsProfileUpdatingBlocked,
            IsCommentPostingBlocked = user.IsCommentPostingBlocked,
            IsReviewPostingBlocked = user.IsReviewPostingBlocked,
            IsPlaylistUploadingBlocked = user.IsPlaylistUploadingBlocked,
            IsPlaylistEditingBlocked = user.IsPlaylistEditingBlocked,
            IsChallengeUploadingBlocked = user.IsChallengeUploadingBlocked,
            IsChallengeScoreUploadingBlocked = user.IsChallengeScoreUploadingBlocked,
        };
    }

    public static IEnumerable<ApiUserPermissionsResponse> FromOldList(IEnumerable<GameUser> oldList, DataContext dataContext)
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}