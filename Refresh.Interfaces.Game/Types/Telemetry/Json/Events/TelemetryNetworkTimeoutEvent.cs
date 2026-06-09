namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryNetworkTimeoutEvent
{
    public string ClientId { get; set; } = "";
    public string HostId { get; set; } = "";
    public TelemetryNetworkTimeoutError Error { get; set; }
}