namespace Refresh.Database.Models.Assets;

/// <summary>
/// Used by the cwlib worker to save the metadata of deserialized plan assets into the DB.
/// Would be useful for moderation, or allowing a user to see their own uploaded items (globally and per-level?)
/// TODO: Ability to request these through the API
/// </summary>
public partial class GameItem
{
    [Key] public string PlanHash { get; set; } = null!;
    public string IconHash { get; set; } = "0";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string CreatorName { get; set; } = "";
    public string[] ContributorNames { get; set; } = [];
    public bool IsGamePhoto { get; set; }
    public bool IsCameraPhoto { get; set; }
    public bool IsUserCreation { get; set; }
}