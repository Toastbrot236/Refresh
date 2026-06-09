namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryObjectiveCompleteEvent
{
    public uint ObjectiveId { get; set; }
    public string ObjctiveName { get; set; } = "";
    public uint Quest { get; set; }
    public uint[] Slot { get; set; } = []; // [slot type, slot id]
}