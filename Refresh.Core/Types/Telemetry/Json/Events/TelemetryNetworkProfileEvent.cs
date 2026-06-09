using Refresh.Core.Types.Matching;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryNetworkProfileEvent
{
    public NatType NatType { get; set; }
    /// <summary>
    /// Sent in the format of "%02x:%02x:%02x:%02x:%02x:%02x"
    /// </summary>
    public string MacAddress { get; set; } = "";
    /// <summary>
    /// Sent in the format of "up:%ld;down:%ld;"
    /// </summary>
    public string NetworkQuality { get; set; } = "";
}