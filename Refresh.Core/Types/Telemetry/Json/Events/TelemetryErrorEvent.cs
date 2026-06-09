namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryErrorEvent
{
    public TelemetryErrorType ErrorType { get; set; }
    public uint ErrorCode { get; set; }
    public string ErrorMessage { get; set; } = "";
}