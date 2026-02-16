namespace Refresh.Database.Models.Assets;

public partial class DisallowedAsset
{
    [Key] public string AssetHash { get; set; } = null!;

    /// <summary>
    /// A short reason on why this hash specifically is blocked (e.g.: inappropriate texture)
    /// </summary>
    public string Reason { get; set; } = "";
    public DateTimeOffset BlockedAt { get; set; }

    /// <summary>
    /// Optional asset type, for moderation/understanding the reason purposes. If the moderator doesn't explicitly
    /// include this info, fall back to the GameAsset's type, incase the asset is already on the server.
    /// This should not affect detecting blocked assets in any way, this is only meant as additional information.
    /// </summary>
    public GameAssetType AssetType { get; set; }
}