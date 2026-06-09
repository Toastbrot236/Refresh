namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCommunityUiEvent
{
    public int NumCategories { get; set; }
    public TelemetryCommunityUiCategory[] InitialCategories { get; set; } = [];
    public string Action { get; set; } = "";
    public uint[] SelectedSlot { get; set; } = [];
    public TelemetryCommunityUiCategory? SelectedCategory { get; set; }
    public uint? SelectedPlaylist { get; set; }
    public string? SelectedUser { get; set; }
    public int SelectedIndex { get; set; }
    [JsonProperty("hscroll")] public int HorizontalScroll { get; set; }
    [JsonProperty("vscroll")] public int VerticalScroll { get; set; }
}