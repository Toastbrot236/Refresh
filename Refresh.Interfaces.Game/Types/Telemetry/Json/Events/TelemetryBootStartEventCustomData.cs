namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryBootStartEventCustomData
{
    /// <summary>
    /// Only sent on PS3
    /// </summary>
    public string? Medium { get; set; }
    /// <summary>
    /// Only sent on PS4
    /// </summary>
    [JsonProperty("PlayGo")] public bool? PlayGo { get; set; }
    public int Ver { get; set; }
}