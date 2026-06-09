namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryStoreMenuScreenEvent
{
    public string StoreSessionId { get; set; } = "";
    public string ScreenId { get; set; } = "";
}