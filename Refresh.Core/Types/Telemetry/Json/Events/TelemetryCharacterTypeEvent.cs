namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCharacterTypeEvent
{
    public TelemetryPlayerCharacter PlayerCharacter { get; set; }
    public int DurationSecs { get; set; }
    public int[] LevelId { get; set; } = [];
    public bool IsChangeling { get; set; }
}