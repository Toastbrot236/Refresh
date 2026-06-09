namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetrySocialInteractionEvent
{
    public TelemetrySocialInteractionType InteractionType { get; set; }
}