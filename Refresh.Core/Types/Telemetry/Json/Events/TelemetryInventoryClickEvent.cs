namespace Refresh.Core.Types.Telemetry.Json.Events;

public class TelemetryInventoryClickEvent
{
    public TelemetryInventoryClickAction Action { get; set; }
    public TelemetryInventoryClickType Type { get; set; }
    public uint[] Guids { get; set; } = [];
    public string[] Hashes { get; set; } = [];
    [JsonProperty("game_id")] public string GameId { get; set; } = "";
    [JsonProperty("slot")] public uint[] Slot { get; set; } = [];
}