namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryDiveInEventEvent
{
    public string PlayerId { get; set; } = "";
    public TelemetryDiveInEvent Event { get; set; }
}