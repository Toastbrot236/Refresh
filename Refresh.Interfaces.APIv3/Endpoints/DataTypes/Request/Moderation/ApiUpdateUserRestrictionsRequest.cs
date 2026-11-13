using Refresh.Database.Query;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Moderation;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUpdateUserRestrictionsRequest : ApiModerationRequest, IApiEditUserPermissions
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
}