namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryModalOsEvent
{
    [JsonProperty("MState")] public string ModalState { get; set; } = "";
    public string GameId { get; set; } = "";
}