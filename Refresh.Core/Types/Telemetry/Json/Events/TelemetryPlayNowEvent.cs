namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayNowEvent
{
    public uint[] Slot { get; set; } = [];
    public TelemetryPlayNowSource Source { get; set; }
}