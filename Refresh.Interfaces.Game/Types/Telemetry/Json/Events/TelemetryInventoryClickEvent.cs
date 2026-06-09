namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

public class TelemetryInventoryClickEvent
{
    public string Action { get; set; } = "";
    public string Type { get; set; } = "";
    public uint[] Guids { get; set; } = [];
    public string[] Hashes { get; set; } = [];
    [JsonProperty("game_id")] public string GameId { get; set; } = "";
    [JsonProperty("slot")] public uint[] Slot { get; set; } = [];
}