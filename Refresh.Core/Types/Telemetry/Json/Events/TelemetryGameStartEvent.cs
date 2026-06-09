namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryGameStartEvent
{
    public string GameId { get; set; } = "";
    public string LevelId { get; set; } = "";
    public uint EntryPoint { get; set; }
    public string Mode { get; set; } = "";
}