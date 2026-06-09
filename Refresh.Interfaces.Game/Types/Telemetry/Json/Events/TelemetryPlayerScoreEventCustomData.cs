namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// this class doesn't specify snake case on purpose
public class TelemetryPlayerScoreEventCustomData
{
    public int Deaths { get; set; }
    public int HighestMult { get; set; }
    public string Mode { get; set; } = "";
}