namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// This struct uses PascalCase intentionally
public class TelemetryTutorVidEvent
{
    public uint Video { get; set; }
    public bool FirstWatch { get; set; }
    public string Title { get; set; } = "";
    public bool PlayedFromTutorialMenu { get; set; }
}