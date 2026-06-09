namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public abstract class TelemetrySearchEvent
{
    public string Text { get; set; } = "";
}

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCommunitySearchEvent : TelemetrySearchEvent;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryStoreSearchEvent : TelemetrySearchEvent;
