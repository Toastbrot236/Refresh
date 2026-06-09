namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

public class TelemetryTrophyAchieveEventCustomData
{
    /// <summary>
    /// The localized name of the trophy
    /// </summary>
    [JsonProperty("localised")] public string Localized { get; set; } = "";
}