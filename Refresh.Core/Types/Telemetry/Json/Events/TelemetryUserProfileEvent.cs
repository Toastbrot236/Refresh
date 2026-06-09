namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryUserProfileEvent
{
    public string NpOnlineId { get; set; } = "";
    public ulong NpAccountId { get; set; }
    public int Age { get; set; }
    public string Region { get; set; } = "";
    public string Language { get; set; } = "";
    public string[] LanguagesUsed { get; set; } = [];
    public bool RestrictChat { get; set; }
    /// <summary>
    /// Whether the profile has user generated media restricted
    /// </summary>
    public bool RestrictUgm { get; set; }
}