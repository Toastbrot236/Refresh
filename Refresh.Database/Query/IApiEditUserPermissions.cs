namespace Refresh.Database.Query;

public interface IApiEditUserPermissions
{
    bool? MayUploadAssets { get; set; }
    bool? MayPublishNewLevels { get; set; }
    bool? MayEditLevels { get; set; }
    bool? MaySetLevelReuploadData { get; set; }
    bool? MayUploadLevelScores { get; set; }
    bool? MayHeartEntities { get; set; }
    bool? MayPublishPhotos { get; set; }
    bool? MayEditProfile { get; set; }
    bool? MayUploadComments { get; set; }
    bool? MayUploadReviews { get; set; }
    bool? MayUploadPlaylists { get; set; }
    bool? MayEditPlaylists { get; set; }
    bool? MayUploadChallenges { get; set; }
    bool? MayUploadChallengeScores { get; set; }
}