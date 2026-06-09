namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLevelHackedEvent // lol, lets see what the game understands as "hacked"
{
    public string LevelId { get; set; } = ""; // in the format of "[%u,%u]"
}