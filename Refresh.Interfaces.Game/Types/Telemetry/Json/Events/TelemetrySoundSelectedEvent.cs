namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

public class TelemetrySoundSelectedEvent
{
    [JsonProperty("SoundName")] public string Name { get; set; } = "";
}