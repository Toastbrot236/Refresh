using Refresh.Core.Types.Data;
using Refresh.Database.Models.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

/// <summary>
/// A user with full information, like current role, ban status, etc.
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiUserRestrictionsResponse : IApiResponse, IDataConvertableFrom<ApiUserRestrictionsResponse, GameUser>
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
    
    public static ApiUserRestrictionsResponse? FromOld(GameUser? user, DataContext dataContext)
    {
        if (user == null) return null;
        
        return new ApiUserRestrictionsResponse
        {
            MayUploadAssets = user.MayUploadAssets,
            MayPublishNewLevels = user.MayPublishNewLevels,
            MayEditLevels = user.MayEditLevels,
            MaySetLevelReuploadData = user.MaySetLevelReuploadData,
            MayUploadLevelScores = user.MayUploadLevelScores,
            MayHeartEntities = user.MayHeartEntities,
            MayPublishPhotos = user.MayPublishPhotos,
            MayEditProfile = user.MayEditProfile,
            MayUploadComments = user.MayUploadComments,
            MayUploadReviews = user.MayUploadReviews,
            MayUploadPlaylists = user.MayUploadPlaylists,
            MayEditPlaylists = user.MayEditPlaylists,
            MayUploadChallenges = user.MayUploadChallenges,
            MayUploadChallengeScores = user.MayUploadChallengeScores,
        };
    }

    public static IEnumerable<ApiUserRestrictionsResponse> FromOldList(IEnumerable<GameUser> oldList, DataContext dataContext)
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}