namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetrySocialInteractionEventCustomData
{
    public string Target { get; set; } = "";
    [JsonProperty("len")] public int Length { get; set; }
}