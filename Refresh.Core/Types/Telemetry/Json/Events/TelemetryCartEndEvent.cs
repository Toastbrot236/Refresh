namespace Refresh.Core.Types.Telemetry.Json.Events;

// This data type encompasses the CartCheckout and CartResult events
[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCartEndEvent
{
    public string StoreSessionId { get; set; } = "";
    public string[] Sku { get; set; } = [];
    public TelemetryCartResultStatusCode? ResultCode { get; set; }
}