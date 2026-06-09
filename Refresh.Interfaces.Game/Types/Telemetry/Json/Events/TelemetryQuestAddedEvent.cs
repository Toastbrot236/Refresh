namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryQuestAddedEvent
{
    public uint QuestId { get; set; }
    public string QuestName { get; set; } = "";
    public TelemetryQuestType QuestType { get; set; }
    public int[] Slot { get; set; } = [];
    [JsonProperty("subobjectives")] public uint[] SubObjectives { get; set; } = [];
    public uint[] Targets { get; set; } = [];
}