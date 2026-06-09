namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryHeatmapPosEvent
{
    public int Player { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public int Z { get; set; }
    public uint FrameCount { get; set; }
    public string Reason { get; set; } = "";
    public uint[] Slot { get; set; } = [];
    public string GameId { get; set; } = "";
}