namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCommunityUiCategory
{
    public string Tag { get; set; } = "";
    public string? Param { get; set; }
}