namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// this class intentionally does not set snake casing
public class TelemetryAnimationSelectedEvent
{
    public uint AnimationGuid { get; set; }
    public TelemetryCharacter Character { get; set; }
}