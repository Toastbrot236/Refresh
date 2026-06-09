namespace Refresh.Core.Types.Telemetry;

public class TelemetryInventoryItem
{
    public uint Action;
    public uint Type;
    public List<uint> Guids = [];
    public List<InlineHash> Hashes = [];
}