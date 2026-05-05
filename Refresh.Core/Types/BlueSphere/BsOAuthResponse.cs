namespace Refresh.Core.Types.BlueSphere;

/// <summary>
/// Referenced from https://bluesphere.live/verify
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class BsOAuthResponse 
{
    public required bool Success { get; set; }

    public required int AccountId { get; set; }
    /// <summary>
    /// The username of the user
    /// </summary>
    public required string OnlineId { get; set; }
    public required string? DiscordId { get; set; }
    public required string UserId { get; set; }
    /// <summary>
    /// The presented name of the game
    /// </summary>
    public required string? CurrentGame { get; set; }
    /// <summary>
    /// Formatted as "1.28", for example
    /// </summary>
    public required string? CurrentGameVersion { get; set; }
}