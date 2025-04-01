namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// A game pin's information, imported using GamePinConfig. May contain more information,
/// but those are not included due to irrelevance to the server.
/// </summary>
public class ImportedPin
{
    public long Id { get; set; }
    public long ProgressType { get; set; }
    public byte Category { get; set; }
    public string? TranslatedName { get; set; }
    public string? TranslatedDescription { get; set; }
    public int TargetValue { get; set; }
}