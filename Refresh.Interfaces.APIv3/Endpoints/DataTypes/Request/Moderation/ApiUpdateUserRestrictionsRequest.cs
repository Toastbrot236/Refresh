using Refresh.Database.Query;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Moderation;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUpdateUserRestrictionsRequest : ApiModerationRequest, IApiEditUserPermissions
{
    public bool? MayUploadAssets { get; set; }
    public bool? MayPublishNewLevels { get; set; }
    public bool? MayEditLevels { get; set; }
    public bool? MaySetLevelReuploadData { get; set; }
    public bool? MayUploadLevelScores { get; set; }
    public bool? MayHeartEntities { get; set; }
    public bool? MayPublishPhotos { get; set; }
    public bool? MayEditProfile { get; set; }
    public bool? MayUploadComments { get; set; }
    public bool? MayUploadReviews { get; set; }
    public bool? MayUploadPlaylists { get; set; }
    public bool? MayEditPlaylists { get; set; }
    public bool? MayUploadChallenges { get; set; }
    public bool? MayUploadChallengeScores { get; set; }
}