using Refresh.Database.Models.Assets;

namespace Refresh.Core.Configuration;

public class RolePermissions
{
    public RolePermissions() {}

    /// <summary>
    /// If this is true, all uploads or edits will be blocked, regardless of the below configs
    /// </summary>
    public bool ReadOnlyMode { get; set; } = false;
    public ConfigAssetFlags BlockedAssetFlags { get; set; } = new(AssetFlags.Dangerous | AssetFlags.Modded);
    public EntityUploadRateLimitProperties LevelUploadRateLimit = new()
    {
        Enabled = false,
        TimeSpanHours = 24,
        UploadQuota = 10,
    };

    public EntityUploadRateLimitProperties PhotoUploadRateLimit = new()
    {
        Enabled = false,
        TimeSpanHours = 24,
        UploadQuota = 10,
    };

    public EntityUploadRateLimitProperties PlaylistUploadRateLimit = new()
    {
        Enabled = false,
        TimeSpanHours = 24,
        UploadQuota = 8,
    };

    /// <summary>
    /// The amount of data the user is allowed to upload before all resource uploads get blocked, defaults to 100mb.
    /// </summary>
    public int UserFilesizeQuota { get; set; } = 100 * 1_048_576;

    // Unlike user-specific perms, these are just bools, because the config options don't have any fallback themselves,
    // while the user perms are supposed to be just overrides of these perms here for specific users, just incase.
    public bool MayUploadAssets { get; set; } = true;
    public bool MayPublishNewLevels { get; set; } = true;
    public bool MayEditLevels { get; set; } = true;
    public bool MaySetLevelReuploadData { get; set; } = true;
    public bool MayUploadLevelScores { get; set; } = true;
    public bool MayHeartEntities { get; set; } = true;
    public bool MayPublishPhotos { get; set; } = true;
    public bool MayEditProfile { get; set; } = true;
    public bool MayUploadComments { get; set; } = true;
    public bool MayUploadReviews { get; set; } = true;
    public bool MayUploadPlaylists { get; set; } = true;
    public bool MayEditPlaylists { get; set; } = true;
    public bool MayUploadChallenges { get; set; } = true;
    public bool MayUploadChallengeScores { get; set; } = true;

    // Not configurable because "Restricted" and "Banned" already imply a user has no uploading perms.
    public static RolePermissions FromRestrictedUser => new()
    {
        ReadOnlyMode = true,
        BlockedAssetFlags = new(AssetFlags.Dangerous | AssetFlags.Modded | AssetFlags.Media),
        // Not enabled because restricted may not upload UGC anyway, also we already enable read-only mode for them
        LevelUploadRateLimit = new()
        {
            Enabled = false,
        },
        PhotoUploadRateLimit = new()
        {
            Enabled = false,
        },
        PlaylistUploadRateLimit = new()
        {
            Enabled = false,
        },
        UserFilesizeQuota = 0,
        // No need to set the perm bools to false, since the read-only one works as a master switch here...
    };
}