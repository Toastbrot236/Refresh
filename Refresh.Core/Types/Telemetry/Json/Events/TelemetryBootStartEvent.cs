namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryBootStartEvent
{
    public TelemetryBootStartType LaunchMethod { get; set; }
    public string BootSessionId { get; set; } = "";
    public bool IsTrial { get; set; }
    public string TitleId { get; set; } = "";
    public TelemetryIssuerId IssuerId { get; set; }
    public string DiscId { get; set; } = "";
    public string BuildVersion { get; set; } = "";
    public string NpCommunicationsId { get; set; } = "";
    public string TitleName { get; set; } = "";
}