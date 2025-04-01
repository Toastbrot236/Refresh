namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// A game pin, converted into from an ImportedPin in GamePinConfig for optimal usage.
/// </summary>
public class GamePin
{
    public long ProgressTypeId { get; set; }
    public byte Category { get; set; }
    public string TranslatedName { get; set; } = "";
    public string TranslatedDescription { get; set; } = "";
    public int[] TargetValues { get; set; } = [];
}