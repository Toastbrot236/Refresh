using System.Diagnostics.CodeAnalysis;
using Bunkum.Core.Configuration;
using Microsoft.CSharp.RuntimeBinder;

namespace Refresh.Core.Configuration;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class GameServerConfig : Config
{
    public override int CurrentConfigVersion => 29;
    public override int Version { get; set; } = 0;
    
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {
        
    }

    protected override void MigrateUsingOtherConfigs(int oldVer, dynamic oldConfig, IEnumerable<Config> otherConfigs)
    {
        foreach (Config config in otherConfigs)
        {
            if (config is not RolePermConfig) continue;

            RolePermConfig rolePermConfig = (RolePermConfig)config;
            
            // In version 27, various (mostly already role-specific) perms, like blocked assets and read-only mode, were moved to dedicated child objects,
            // to more cleanly split the perms between certain roles, and to make their enforcement easier.
            if (oldVer < 27)
            {
                rolePermConfig.NormalUserPermissions = new();
                rolePermConfig.TrustedUserPermissions = new();

                // filesize quota limit was added during version 11, but the version wasn't bumped, so catch error to be safe
                // Migrate filesize quota
                if (oldVer >= 11)
                {
                    try
                    {
                        rolePermConfig.NormalUserPermissions.UserFilesizeQuota = (int)oldConfig.UserFilesizeQuota;
                        rolePermConfig.TrustedUserPermissions.UserFilesizeQuota = (int)oldConfig.UserFilesizeQuota;
                    }
                    catch (RuntimeBinderException)
                    {
                        // do nothing
                    }
                }

                // Migrate asset flags/safety level
                if (oldVer >= 18)
                {
                    rolePermConfig.NormalUserPermissions.BlockedAssetFlags.Dangerous = (bool)oldConfig.BlockedAssetFlags.Dangerous;
                    rolePermConfig.NormalUserPermissions.BlockedAssetFlags.Media = (bool)oldConfig.BlockedAssetFlags.Media;
                    rolePermConfig.NormalUserPermissions.BlockedAssetFlags.Modded = (bool)oldConfig.BlockedAssetFlags.Modded;

                    rolePermConfig.TrustedUserPermissions.BlockedAssetFlags.Dangerous = (bool)oldConfig.BlockedAssetFlagsForTrustedUsers.Dangerous;
                    rolePermConfig.TrustedUserPermissions.BlockedAssetFlags.Media = (bool)oldConfig.BlockedAssetFlagsForTrustedUsers.Media;
                    rolePermConfig.TrustedUserPermissions.BlockedAssetFlags.Modded = (bool)oldConfig.BlockedAssetFlagsForTrustedUsers.Modded;
                }
                else
                {
                    // Asset safety level was added in config version 2, so dont try to migrate if we are coming from an older version than that
                    if (oldVer >= 2)
                    {
                        int oldSafetyLevel = (int)oldConfig.MaximumAssetSafetyLevel;
                        ConfigAssetFlags fromSafetyLevel = new ConfigAssetFlags
                        {
                            Dangerous = oldSafetyLevel < 3,
                            Modded = oldSafetyLevel < 2,
                            Media = oldSafetyLevel < 1,
                        };
                        rolePermConfig.NormalUserPermissions.BlockedAssetFlags = fromSafetyLevel;
                    }

                    // Asset safety level for trusted users was added in config version 12, so dont try to migrate if we are coming from a version older than that
                    if (oldVer >= 12)
                    {
                        // There was no version bump for trusted users being added, so we just have to catch this error :/
                        try
                        {
                            int oldTrustedSafetyLevel = (int)oldConfig.MaximumAssetSafetyLevelForTrustedUsers;
                            rolePermConfig.TrustedUserPermissions.BlockedAssetFlags = new ConfigAssetFlags
                            {
                                Dangerous = oldTrustedSafetyLevel < 3,
                                Modded = oldTrustedSafetyLevel < 2,
                                Media = oldTrustedSafetyLevel < 1,
                            };
                        }
                        catch (RuntimeBinderException)
                        {
                            rolePermConfig.TrustedUserPermissions.BlockedAssetFlags = rolePermConfig.NormalUserPermissions.BlockedAssetFlags;
                        }
                    }
                }

                // Timed level upload limits were added in version 19.
                // Migrate level limits
                if (oldVer >= 19)
                {
                    rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.Enabled = (bool)oldConfig.TimedLevelUploadLimits.Enabled;
                    rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.TimeSpanHours = (int)oldConfig.TimedLevelUploadLimits.TimeSpanHours;
                    rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.UploadQuota = (int)oldConfig.TimedLevelUploadLimits.LevelQuota;

                    rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.Enabled = (bool)oldConfig.TimedLevelUploadLimits.Enabled;
                    rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.TimeSpanHours = (int)oldConfig.TimedLevelUploadLimits.TimeSpanHours;
                    rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.UploadQuota = (int)oldConfig.TimedLevelUploadLimits.LevelQuota;
                }

                // Read-only mode was added for both normal and trusted users in version 20.
                if (oldVer >= 20)
                {
                    rolePermConfig.NormalUserPermissions.ReadOnlyMode = (bool)oldConfig.ReadOnlyMode;
                    rolePermConfig.TrustedUserPermissions.ReadOnlyMode = (bool)oldConfig.ReadonlyModeForTrustedUsers;
                }
            }

            // In version 28, PhotoUploadRateLimit and PlaylistUploadRateLimit were added to RolePermissions
            // and various attributes related to level rate-limiting were renamed
            else if (oldVer == 27)
            {
                rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.Enabled = (bool)oldConfig.NormalUserPermissions.TimedLevelUploadLimits.Enabled;
                rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.TimeSpanHours = (int)oldConfig.NormalUserPermissions.TimedLevelUploadLimits.TimeSpanHours;
                rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.UploadQuota = (int)oldConfig.NormalUserPermissions.TimedLevelUploadLimits.LevelQuota;

                rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.Enabled = (bool)oldConfig.TrustedUserPermissions.TimedLevelUploadLimits.Enabled;
                rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.TimeSpanHours = (int)oldConfig.TrustedUserPermissions.TimedLevelUploadLimits.TimeSpanHours;
                rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.UploadQuota = (int)oldConfig.TrustedUserPermissions.TimedLevelUploadLimits.LevelQuota;
            }
            
            // In version 29, the NewUser role and its related config options
            // (NewUserPermissions and HoursUntilNewAccountNoLongerNew) were added.
            // At this point, NormalUserPermissions will already be fully migrated, so we can just copy it.
            if (oldVer < 29)
            {
                rolePermConfig.NewUserPermissions = rolePermConfig.NormalUserPermissions;
            }
            // In version 30, all role-perm related configs were moved to rolePermissions.json
            if (oldVer == 29)
            {
                
                rolePermConfig.NewUserPermissions = oldConfig.NewUserPermissions;
                rolePermConfig.NormalUserPermissions = oldConfig.NormalUserPermissions;
                rolePermConfig.TrustedUserPermissions = oldConfig.TrustedUserPermissions;
                rolePermConfig.HoursUntilNewAccountNoLongerNew = oldConfig.HoursUntilNewAccountNoLongerNew;
            }

            rolePermConfig.SaveToJsonFile();
            break;
        }
    }

    public string LicenseText { get; set; } = "Welcome to Refresh!";
    
    public bool AllowUsersToUseIpAuthentication { get; set; } = false;
    public bool PermitPsnLogin { get; set; } = true;
    public bool PermitRpcnLogin { get; set; } = true;
    public bool PermitWebLogin { get; set; } = true;
    /// <summary>
    /// Secondary safety switch incase the PSN and RPCN toggles somehow fail.
    /// </summary>
    public bool PermitAllLogins { get; set; } = true;

    /// <summary>
    /// Should all game logins be required to use Patchwork?
    /// </summary>
    public bool EnforcePatchwork { get; set; } = true;

    /// <summary>
    /// The minimum required major version of Patchwork on the client to be able to connect.
    /// </summary>
    public int RequiredPatchworkMajorVersion { get; set; } = 1;
    
    /// <summary>
    /// The minimum required minor version of Patchwork on the client to be able to connect.
    /// </summary>
    public int RequiredPatchworkMinorVersion { get; set; } = 0;
    
    public bool UseTicketVerification { get; set; } = true;
    public bool RegistrationEnabled { get; set; } = true;
    public string InstanceName { get; set; } = "Refresh";
    public string InstanceDescription { get; set; } = "A server running Refresh!";
    public bool MaintenanceMode { get; set; } = false;
    public bool RequireGameLoginToRegister { get; set; } = false;
    /// <summary>
    /// Whether to use deflate compression for responses.
    /// If this is disabled, large enough responses will cause LBP to overflow its read buffer and eventually corrupt its own memory to the point of crashing.
    /// </summary>
    public bool UseDeflateCompression { get; set; } = true;
    public string WebExternalUrl { get; set; } = "https://refresh.example.com";
    /// <summary>
    /// The base URL that LBP3 uses to grab config files like `network_settings.nws`.
    /// </summary>
    public string GameConfigStorageUrl { get; set; } = "https://refresh.example.com/lbp";
    public bool AllowInvalidTextureGuids { get; set; } = false;
    
    /// <summary>
    /// Whether to print the room state whenever a `FindBestRoom` match returns no results
    /// </summary>
    public bool PrintRoomStateWhenNoFoundRooms { get; set; } = true;

    /// <summary>
    /// Whether to unconditionally print data like token, token owner, remote IP, request URI etc during authentication outside of exceptions
    /// </summary>
    public bool PrintAuthenticationData { get; set; } = false;

    public string[] Sha1DigestKeys = ["CustomServerDigest"];
    public string[] HmacDigestKeys = ["CustomServerDigest"];

    public bool PermitShowingOnlineUsers { get; set; } = true;
    public bool EnableDiveIn { get; set; } = true;
}