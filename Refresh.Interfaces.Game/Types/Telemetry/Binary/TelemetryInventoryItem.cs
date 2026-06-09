namespace Refresh.Interfaces.Game.Types.Telemetry.Binary;

public class TelemetryInventoryItem
{
    public uint Action { get; set; }
    public uint Type { get; set; }
    public List<uint> Guids { get; set; } = [];
    public List<InlineHash> Hashes { get; set; } = [];
}