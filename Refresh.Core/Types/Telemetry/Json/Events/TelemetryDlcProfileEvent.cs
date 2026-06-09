namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryDlcProfileEvent
{
    public string[] Sku { get; set; } = [];
}