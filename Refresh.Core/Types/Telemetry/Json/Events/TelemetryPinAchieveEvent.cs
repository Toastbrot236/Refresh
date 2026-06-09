namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPinAchieveEvent
{
    public TelemetryPinSetVersion PinSetVersion { get; set; }
    public bool IsFirstTime { get; set; }
    public TelemetryPinType PinType { get; set; }
    public uint PinId { get; set; }
}