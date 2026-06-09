namespace Refresh.Core.Types.Telemetry.Json.Events;

public class TelemetryPingEvent
{
    public string User { get; set; } = "";
    public float PacketLoss { get; set; }
    [JsonProperty("RTTMs")] public float RoundTripDelay { get; set; }
    [JsonProperty("BwAvailKBPS")] public float BandwidthAvailableKbps { get; set; }
    [JsonProperty("BwNPAvailKBPS")] public float BandwidthNetplayAvailableKbps { get; set; }
    [JsonProperty("BwUsedKBPS")] public float BandwidthUsedKbps { get; set; }
}