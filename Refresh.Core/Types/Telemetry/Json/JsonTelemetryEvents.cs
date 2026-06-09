namespace Refresh.Core.Types.Telemetry.Json;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class JsonTelemetryEvents
{
    public int Count { get; set; }
    public JsonTelemetryEvent[] Events { get; set; } = [];
}