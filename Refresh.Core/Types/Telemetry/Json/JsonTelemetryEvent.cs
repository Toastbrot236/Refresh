using Newtonsoft.Json.Linq;

namespace Refresh.Core.Types.Telemetry.Json;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class JsonTelemetryEvent
{
    public JsonTelemetryHeader Header { get; set; } = null!;
    public JObject Data { get; set; } = null!;
    public JObject? CustomData { get; set; }
}