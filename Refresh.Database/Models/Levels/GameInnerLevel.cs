namespace Refresh.Database.Models.Levels;

/// <summary>
/// Models a level inside an adventure
/// </summary>
[PrimaryKey(nameof(InnerId), nameof(AdventureId))]
public class GameInnerLevel
{
    /// <summary>
    /// The ID of this inner level, unique inside the adventure
    /// </summary>
    [Required] public int InnerId { get; set; }
    
    /// <summary>
    /// The adventure this level is in
    /// </summary>
    [ForeignKey(nameof(AdventureId)), Required] public GameLevel Adventure { get; set; } = null!;
    /// <summary>
    /// The ID of the adventure this level is in
    /// </summary>
    [Required] public int AdventureId { get; set; }
    
    /// <summary>
    /// When this inner level was added to DB
    /// </summary>
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// When this inner level's metadata was last updated
    /// </summary>
    public DateTimeOffset MetadataUpdatedAt { get; set; }
    /// <summary>
    /// When this inner level's root resource was last updated
    /// </summary>
    /// <remarks>
    /// This can be slightly delayed from MetadataUpdatedAt, since when the adventure gets republished,
    /// the cwlib-worker is supposed to find out which of the inner levels had their root resources updated.
    /// </remarks>
    public DateTimeOffset RootResourceUpdatedAt { get; set; }

    /// <summary>
    /// Whether this level was already included in the /publish request body, or was purely
    /// found by the cwlib-worker (might hint at a LBP3 bug in case any such bug is ever found)
    /// </summary>
    public bool DiscoveredFromPublish { get; set; } = true;
    
    public string Title { get; set; } = "";
    public string IconHash { get; set; } = "";
    public string Description { get; set; } = "";
    public GameLevelType LevelType { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool EnforceMinMaxPlayers { get; set; }
    public bool RequiresMoveController { get; set; }
    public List<Label> Labels { get; set; } = [];
    
    // The attributes below are, for some reason, never included in the adventure publish body,
    // and instead have to be obtained by deserializing the adventure's dependencies using the cwlib-worker.
    // The only important attribute here is the root resource hash, but since we already need deserialization
    // to be able to assign a particular hash to a particular inner level ID,
    // we can aswell also take these other potentially interesting attributes.
    // 
    // If any attribute in this region is null, the worker hasn't processed and set it yet.
    #region Cwlib-Exclusive
    public string RootResource { get; set; } = "";
    public float LocationX { get; set; }
    public float LocationY { get; set; }
    public float LocationZ { get; set; }
    public byte BadgeSize { get; set; }
    public List<string> ContributorNames { get; set; } = [];

    // Can be determined without the worker, but we don't know which ID, and therefore which dataset to
    // attribute a particular level asset's modded status to, so we do still partially rely on the worker for this.
    public bool IsModded { get; set; } = false;
    #endregion
}