namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerLeaveEvent
{
    public string GameId { get; set; } = "";
    public string NpOnlineId { get; set; } = "";
    /// <summary>
    /// Format of "[%u,%u]"
    /// </summary>
    public string LevelId { get; set; } = "";
    public uint DurationSecs { get; set; }
    public string Outcome { get; set; } = "";
    public string Mode { get; set; } = "";
    public int PlayerCount { get; set; }
}