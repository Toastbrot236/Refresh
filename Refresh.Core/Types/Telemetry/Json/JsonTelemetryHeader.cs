namespace Refresh.Core.Types.Telemetry.Json;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class JsonTelemetryHeader
{
    public string UserId { get; set; } = "";
    public string TitleId { get; set; } = "";
    public string SessionId { get; set; } = "";
    public long ClientTimestamp { get; set; }
    public string ClientTimezone { get; set; } = "";
}