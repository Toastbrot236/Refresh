namespace Refresh.Core.Types.Telemetry.Json.Events;

public class TelemetryNetStats
{
    [JsonProperty("IsLocal")] public bool IsLocal { get; set; }
    [JsonProperty("Player")] public string Player { get; set; } = "";
    [JsonProperty("AvailBW")] public uint AvailableBandwidth { get; set; }
    [JsonProperty("AvailRNPBW")] public uint AvailableRnpBandwidth { get; set; }
    [JsonProperty("AvailGameBW")] public float AvailableGameBandwidth { get; set; }
    [JsonProperty("RecentTotBWUsed")] public uint RecentTotalBandwidthUsed { get; set; }
    [JsonProperty("TimeBtwnSends")] public float TimeBetweenSends { get; set; }
}