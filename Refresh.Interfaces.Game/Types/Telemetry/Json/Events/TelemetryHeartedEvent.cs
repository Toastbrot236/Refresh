namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// this class intentionally does not specify snake case
public class TelemetryGenericHeartedEvent
{
    public int Hearted { get; set; }
    public TelemetryHeartedType Type { get; set; }
}

public class TelemetryUserHeartedEventData
{
    public int Hearted { get; set; }
    public string User { get; set; } = "";
    public string Meta { get; set; } = "";
}

public class TelemetrySlotHeartedEventData
{
    public int Hearted { get; set; }
    public uint[] Level { get; set; } = [];
    public string LevelOwner { get; set; } = "";
    public string Meta { get; set; } = "";
}

public class TelemetryPlaylistHeartedEventData
{
    public int Hearted { get; set; }
    public uint Playlist { get; set; }
}

public class TelemetryItemHeartedEventData
{
    public int Hearted { get; set; }
    [JsonProperty("uid")] public uint Uid { get; set; }
    [JsonProperty("guid")] public uint Guid { get; set; }
}