namespace Refresh.Database.Models.Assets;

public partial class DisallowedAsset
{
    [Key] public string AssetHash { get; set; } = null!;

    /// <summary>
    /// A short reason on why this hash specifically is blocked (e.g.: inappropriate texture)
    /// </summary>
    public string Reason { get; set; } = "";
    public DateTimeOffset BlockedAt{ get; set; }
}