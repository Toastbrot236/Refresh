namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryNetworkJoinResultEvent
{
    public TelemetryNetworkJoinResultReporter Reporter { get; set; }
    public string ClientId { get; set; } = "";
    public string HostId { get; set; } = "";
    public TelemetryNetworkJoinResultResponse Response { get; set; }
    public TelemetryNetworkJoinResultReason Reason { get; set; }
}