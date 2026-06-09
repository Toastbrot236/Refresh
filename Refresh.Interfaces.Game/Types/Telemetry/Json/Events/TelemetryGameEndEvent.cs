namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryGameEndEvent
{
    public uint PlayerCount { get; set; }
    public uint DurationSecs { get; set; }
    public uint LocalPlayerCount { get; set; }
    public string LevelId { get; set; } = "";
    public TelemetryGameEndReason EndReason { get; set; }
    public string Mode { get; set; } = "";
    public string GameId { get; set; } = "";
    public bool IsCompleted { get; set; }
}