namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryTrophyAchieveEvent
{
    public TelemetryHardwareProfileConsole TrophySetVersion { get; set; }
    public bool IsFirstTime { get; set; }
    public char TrophyType { get; set; }
    public int TrophyId { get; set; }
}