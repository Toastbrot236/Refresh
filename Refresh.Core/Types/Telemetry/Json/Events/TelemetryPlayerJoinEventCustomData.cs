namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerJoinEventCustomData
{
    /// <summary>
    /// Format of "[%u,%u]"
    /// </summary>
    public string LevelId { get; set; } = "";
    public uint GameTime { get; set; }
    public int Number { get; set; }
    [JsonProperty("HowMatched")] public TelemetryPlayerJoinHowMatched HowMatched { get; set; }
}