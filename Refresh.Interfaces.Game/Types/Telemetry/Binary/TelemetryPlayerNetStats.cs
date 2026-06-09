namespace Refresh.Interfaces.Game.Types.Telemetry.Binary;

public class TelemetryPlayerNetStats
{
    public uint Frame { get; set; }
    public uint Player { get; set; }
    public bool IsLocal { get; set; }
    public uint AvailableBandwidth { get; set; }
    public uint AvailableRnpBandwidth { get; set; }
    public float AvailableGameBandwidth { get; set; }
    public uint RecentTotalBandwidthUsed { get; set; }
    public float TimeBetweenSends { get; set; }
}