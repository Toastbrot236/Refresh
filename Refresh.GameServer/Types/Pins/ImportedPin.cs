namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// A game pin's information, imported from GamePinConfig. May contain more information,
/// but those are not included due to irrelevance to the server.
/// </summary>
/// <remarks>
/// Modelled after the file contents in https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json
/// and https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbpvita.json
/// </remarks>
public class ImportedPin
{
    public long ProgressType { get; set; }
    public byte Category { get; set; }
    public string? TranslatedName { get; set; }
    public string? TranslatedDescription { get; set; }
    public int TargetValue { get; set; }
}