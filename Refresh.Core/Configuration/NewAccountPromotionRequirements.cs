namespace Refresh.Core.Configuration;

/// <summary>
/// This holds all requirements a user with the NewUser role must meet before we
/// automatically upgrade their role to User, potentially giving them higher perms
/// (depends on RolePermissions in the config).
/// </summary>
public class NewAccountPromotionRequirements
{
    // TODO maybe also weigh playtime/plays/completions depending on slot type and platform?
    // TODO reconsider defaults
    
    /// <summary>
    /// How old the user's account must be.
    /// </summary>
    public int AccountAgeHours { get; set; } = 24 * 7;

    /// <summary>
    /// How much play time the user needs across any game or platform.
    /// </summary>
    public int ActivePlayTimeHours { get; set; } = 4;

    /// <summary>
    /// How many total levels the user has to play across any game or platform.
    /// </summary>
    public int TotalLevelPlays { get; set; } = 20;

    /// <summary>
    /// How many total levels the user has to complete (get a score in) across any game or platform.
    /// </summary>
    public int TotalLevelCompletions { get; set; } = 20;

    /// <summary>
    /// How many unique levels the user has to play across any game or platform.
    /// </summary>
    public int UniqueLevelPlays { get; set; } = 10;

    /// <summary>
    /// How many unique levels the user has to complete (get a score in) across any game or platform.
    /// </summary>
    public int UniqueLevelCompletions { get; set; } = 10;
}