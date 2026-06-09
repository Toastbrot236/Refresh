namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryStoreEndEvent
{
    public string StoreSessionId { get; set; } = "";
    public uint DurationSecs { get; set; }
    public bool DidPurchase { get; set; } // serialized as single byte 0 or 1
}