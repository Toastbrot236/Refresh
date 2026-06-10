namespace Refresh.Interfaces.Game.Types.Telemetry.Binary;

public struct TelemetryHeader
{
    public ushort Revision { get; set; }
    public uint HashedPlayerId { get; set; }
    public InlineHash LevelHash { get; set; }
    public uint SlotType { get; set; }
    public uint SlotNumber { get; set; }

    public bool HasFullHash { get; set; }
    public bool HasTimestamps { get; set; }
}