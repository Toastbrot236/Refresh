namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryQuestCompletedEvent
{
    public uint QuestId { get; set; }
    public string QuestName { get; set; } = "";
    public uint[] Slot { get; set; } = [];
}