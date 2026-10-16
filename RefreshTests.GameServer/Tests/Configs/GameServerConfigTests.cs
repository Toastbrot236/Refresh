using Refresh.Core.Configuration;
using Refresh.Database.Models.Assets;
using RefreshTests.GameServer.GameServer.Configuration;

namespace RefreshTests.GameServer.Tests.Configs;

public class GameServerConfigTests : GameServerTest
{
    [Test]
    public void MigratesRolePermsFromVersion26()
    {
        TestGameServerConfig config = new()
        {
            Version = 26,
            ReadOnlyMode = true,
            ReadonlyModeForTrustedUsers = false,
            TimedLevelUploadLimits = new()
            {
                Enabled = true,
                TimeSpanHours = 67,
                LevelQuota = 2,
            },
            BlockedAssetFlags = new(AssetFlags.Dangerous | AssetFlags.Media),
            BlockedAssetFlagsForTrustedUsers = new(AssetFlags.Modded),
            UserFilesizeQuota = 141,
        };

        RolePermConfig rolePermConfig = new();
        config.TestMigration(rolePermConfig);

        // new is now the same as normal, and all config values were moved to RolePermConfig
        Assert.That(rolePermConfig.NewUserPermissions.ReadOnlyMode, Is.True);
        Assert.That(rolePermConfig.NormalUserPermissions.ReadOnlyMode, Is.True);
        Assert.That(rolePermConfig.TrustedUserPermissions.ReadOnlyMode, Is.False);

        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.Enabled, Is.True);
        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.Enabled, Is.True);
        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.Enabled, Is.True);

        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(67));
        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(67));
        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(67));

        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(2));
        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(2));
        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(2));

        Assert.That(rolePermConfig.NewUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Dangerous | AssetFlags.Media));
        Assert.That(rolePermConfig.NormalUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Dangerous | AssetFlags.Media));
        Assert.That(rolePermConfig.TrustedUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Modded));

        Assert.That(rolePermConfig.NewUserPermissions.UserFilesizeQuota, Is.EqualTo(141));
        Assert.That(rolePermConfig.NormalUserPermissions.UserFilesizeQuota, Is.EqualTo(141));
        Assert.That(rolePermConfig.TrustedUserPermissions.UserFilesizeQuota, Is.EqualTo(141));
    }

    [Test]
    public void MigratesEntityUploadRateLimitsFromVersion27()
    {
        TestGameServerConfig config = new()
        {
            Version = 27,
            NormalUserPermissions = new TestRolePermissions()
            {
                TimedLevelUploadLimits = new()
                {
                    Enabled = true,
                    TimeSpanHours = 1234567,
                    LevelQuota = 852094,
                },
            },
            TrustedUserPermissions = new TestRolePermissions()
            {
                TimedLevelUploadLimits = new()
                {
                    Enabled = false,
                    TimeSpanHours = 230,
                    LevelQuota = 7122036,
                },
            },
        };

        RolePermConfig rolePermConfig = new();
        config.TestMigration(rolePermConfig);

        // Same as normal, and all config values were moved to RolePermConfig
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.Enabled, Is.True);
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(1234567));
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(852094));

        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.Enabled, Is.True);
        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(1234567));
        Assert.That(rolePermConfig.NormalUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(852094));

        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.Enabled, Is.False);
        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(230));
        Assert.That(rolePermConfig.TrustedUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(7122036));
    }

    [Test]
    public void MigratesRolePermsFromVersion17()
    {
        TestGameServerConfig config = new()
        {
            Version = 17,
            MaximumAssetSafetyLevel = 1,
            MaximumAssetSafetyLevelForTrustedUsers = 2,
        };

        RolePermConfig rolePermConfig = new();
        config.TestMigration(rolePermConfig);

        // All (migrated) config values were all moved to RolePermConfig
        Assert.That(rolePermConfig.NewUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Dangerous | AssetFlags.Modded)); // same as normal
        Assert.That(rolePermConfig.NormalUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Dangerous | AssetFlags.Modded));
        Assert.That(rolePermConfig.TrustedUserPermissions.BlockedAssetFlags.ToAssetFlags(), Is.EqualTo(AssetFlags.Dangerous));
    }

    [Test]
    public void MigratesNewUserPermsFromVersion28()
    {
        TestGameServerConfig config = new()
        {
            Version = 28,
            NormalUserPermissions = new()
            {
                LevelUploadRateLimit = new()
                {
                    Enabled = true,
                    TimeSpanHours = 22222,
                    UploadQuota = 4,
                },
                BlockedAssetFlags = new(AssetFlags.Dangerous | AssetFlags.Modded),
                ReadOnlyMode = true,
            },
        };
        RolePermConfig rolePermConfig = new();
        config.TestMigration(rolePermConfig);
        
        // Ensure it's exactly the same as normal user now, and all config values were moved to RolePermConfig
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.Enabled, Is.True);
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.TimeSpanHours, Is.EqualTo(22222));
        Assert.That(rolePermConfig.NewUserPermissions.LevelUploadRateLimit.UploadQuota, Is.EqualTo(4));

        Assert.That(rolePermConfig.NewUserPermissions.BlockedAssetFlags.Dangerous, Is.True);
        Assert.That(rolePermConfig.NewUserPermissions.BlockedAssetFlags.Modded, Is.True);
        Assert.That(rolePermConfig.NewUserPermissions.BlockedAssetFlags.Media, Is.False);

        Assert.That(rolePermConfig.NewUserPermissions.ReadOnlyMode, Is.True);
    }
}