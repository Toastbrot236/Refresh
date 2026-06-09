namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerJoinEvent
{
    public string GameId { get; set; } = "";
    public string NpOnlineId { get; set; } = "";
    /// <summary>
    /// Either guest:ABCDEFGH, owner, or host
    /// </summary>
    public string PlayerType { get; set; } = "";
}