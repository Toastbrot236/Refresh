namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryTakePhotoEvent
{
    [JsonProperty("Action")] public TelemetryTakePhotoAction Action { get; set; }
    public string GameId { get; set; } = "";
}