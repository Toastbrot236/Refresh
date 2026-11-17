namespace Refresh.Database.Models.Users;

// APIv4 will have all its enum values serialized to string anyway, so whatever
public enum UserPermissionSetting : sbyte
{
    /// <summary>
    /// This permission is explicitly allowed, even if it's otherwise disallowed by config.
    /// </summary>
    ExplicitlyAllowed = 1,

    /// <summary>
    /// This permission is not explicitly set, so use config value.
    /// </summary>
    UseDefault = 0,

    /// <summary>
    /// This permission is explicitly blocked, even if it's otherwise allowed by config.
    /// </summary>
    ExplicitlyBlocked = -1,
}