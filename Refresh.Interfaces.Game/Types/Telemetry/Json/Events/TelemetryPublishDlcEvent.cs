namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// this class intentionally does not set snake casing
public class TelemetryPublishDlcEvent
{
    public uint PublishGuid { get; set; }
    public string PublishHash { get; set; } = "";
    [JsonProperty("actionTaken")] public bool ActionTaken { get; set; }
}