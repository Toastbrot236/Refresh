namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class TelemetryResourceErrorEvent
{
    public string Hash { get; set; } = "";
    public uint Guid { get; set; }
    public TelemetryResourceErrorType ErrorType { get; set; }
}