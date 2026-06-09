namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryFriendProfileEvent
{
    /// <summary>
    /// An array of online IDs (*not* usernames!) that the user has friended
    /// </summary>
    public string[] FriendList { get; set; } = [];

    /// <summary>
    /// An array of online IDs (*not* usernames!) that the user has blocked
    /// </summary>
    public string[] BlockList { get; set; } = [];
}