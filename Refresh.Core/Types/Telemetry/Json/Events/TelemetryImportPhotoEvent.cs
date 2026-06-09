namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryImportPhotoEvent
{
    public uint Uid { get; set; }
    public string Hash { get; set; } = "";
    public string Fname { get; set; } = "";
}