namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLiveStreamBaseEvent
{
    public string StreamId { get; set; } = "";
    public ulong Tag { get; set; }
    [JsonProperty("levelhash")] public string LevelHash { get; set; } = "";
    public uint[] Slot { get; set; } = [];
}

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLiveStreamEndEvent : TelemetryLiveStreamBaseEvent
{
    public uint StreamLengthSecs { get; set; }
    public uint CurrentViewers { get; set; }
    public uint PeakViewers { get; set; }
}

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLiveStreamDataEvent : TelemetryLiveStreamBaseEvent
{
    public bool? Interactive { get; set; }
    public string[]? InteractiveCommandWords { get; set; }
}

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLiveStreamUpdateEvent : TelemetryLiveStreamDataEvent
{
    public uint CurrentViewers { get; set; }
}

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLiveStreamStartEvent : TelemetryLiveStreamDataEvent
{
    public string Name { get; set; } = "";
    public string StreamService { get; set; } = ""; // TODO: what services can this be?
}