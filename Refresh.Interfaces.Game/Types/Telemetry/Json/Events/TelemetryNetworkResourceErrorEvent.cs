namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

public class TelemetryNetworkResourceErrorEvent
{
    [JsonProperty("hash")] public string Hash { get; set; } = "";
    [JsonProperty("guid")] public uint Guid { get; set; }
    [JsonProperty("networkErrorType")] public TelemetryNetworkResourceErrorType NetworkErrorType { get; set; }
    [JsonProperty("HashedPlayerID")] public string HashedPlayerId { get; set; } = "";
}