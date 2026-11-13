namespace Refresh.Database.Query;

public interface IApiEditUserPermissions
{
    bool? IsAssetUploadingBlocked { get; set; }
    bool? IsLevelPublishingBlocked { get; set; }
    bool? IsLevelEditingBlocked { get; set; }
    bool? IsLevelReattributingBlocked { get; set; }
    bool? IsLevelScoreUploadingBlocked { get; set; }
    bool? IsHeartingBlocked { get; set; }
    bool? IsPhotoPublishingBlocked { get; set; }
    bool? IsProfileUpdatingBlocked { get; set; }
    bool? IsCommentPostingBlocked { get; set; }
    bool? IsReviewPostingBlocked { get; set; }
    bool? IsPlaylistUploadingBlocked { get; set; }
    bool? IsPlaylistEditingBlocked { get; set; }
    bool? IsChallengeUploadingBlocked { get; set; }
    bool? IsChallengeScoreUploadingBlocked { get; set; }
}