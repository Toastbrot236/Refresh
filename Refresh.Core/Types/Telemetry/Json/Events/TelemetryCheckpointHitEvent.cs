namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCheckpointHitEvent
{
    public int[] Slot { get; set; } = [];
    public bool Respawn { get; set; }
    public int CheckpointId { get; set; }
}