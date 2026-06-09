namespace Refresh.Core.Types.Telemetry;

public struct TelemetryHeader
{
    public ushort Revision;
    public uint HashedPlayerId;
    public InlineHash LevelHash;
    public uint SlotType;
    public uint SlotNumber;
}