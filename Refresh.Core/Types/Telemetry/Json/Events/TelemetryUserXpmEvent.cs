namespace Refresh.Core.Types.Telemetry.Json.Events;

public class TelemetryUserXpmEvent
{
    [JsonProperty("CurMSPF")] public float CurMspf { get; set; }
    [JsonProperty("AvgMSPF")] public float AvgMspf { get; set; }
    [JsonProperty("HiMSPF")] public float HiMspf { get; set; }
    [JsonProperty("PredictApplied")] public uint PredictApplied { get; set; }
    [JsonProperty("PredictDesired")] public uint PredictDesired { get; set; }
    [JsonProperty("IsHost")] public bool IsHost { get; set; }
    [JsonProperty("IsCreate")] public bool IsCreate { get; set; }
    [JsonProperty("NumPlayers")] public uint NumPlayers { get; set; }
    [JsonProperty("NumPS3s")] public uint NumPs3s { get; set; }
    [JsonProperty("AvgRTTHost")] public float AvgRttHost { get; set; }
    [JsonProperty("BWUsage")] public float BandwidthUsage { get; set; }
    [JsonProperty("WorstPing")] public float WorstPing { get; set; }
    [JsonProperty("WorstBW")] public float WorstBandwidth { get; set; }
    [JsonProperty("WorstPL")] public float WorstPl { get; set; }
    [JsonProperty("WorstPlayers")] public uint WorstPlayers { get; set; }
    [JsonProperty("HTTPBWUp")] public float HttpBandwidthUp { get; set; }
    [JsonProperty("HTTPBWDown")] public float HttpBandwidthDown { get; set; }
    [JsonProperty("Frame")] public uint Frame { get; set; }
    [JsonProperty("LastMGJFrame")] public uint LastMgjFrame { get; set; }

    [JsonProperty("NetStats")] public TelemetryNetStats[] TelemetryNetworkStats { get; set; } = [];
}