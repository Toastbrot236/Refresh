namespace Refresh.Database.Models.Levels;

/// <summary>
/// Models a level inside an adventure. This is an independent entity because it must also have a
/// reference to a GameLevel (its parent adventure) and to not interfere with any other "actual" levels.
/// </summary>
[PrimaryKey(nameof(InnerLevelId), nameof(AdventureId))]
public partial class GameAdventureLevel
{
    public byte InnerLevelId { get; set; }
    [Required] public int AdventureId { get; set; }

    /// <summary>
    /// The parent adventure this level belongs to
    /// </summary>
    [Required, ForeignKey(nameof(AdventureId))] 
    public GameLevel Adventure { get; set; } = null!;

    public string Title { get; set; } = "";
    public string IconHash { get; set; } = "0";
    public string Description { get; set; } = "";

    public int LocationX { get; set; }
    public int LocationY { get; set; }

    public string RootResource { get; set; } = string.Empty;

    /// <summary>
    /// When the level was first included in an adventure publish
    /// </summary>
    public DateTimeOffset PublishDate { get; set; }
    /// <summary>
    /// The last adventure publish which included this level
    /// </summary>
    public DateTimeOffset UpdateDate { get; set; }
    
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool EnforceMinMaxPlayers { get; set; }
    
    public GameLevelType LevelType { get; set; }

    /// <summary>
    /// The associated ID for the inner developer adventure level.
    /// </summary>
    public int StorySubId { get; set; }

    public GameSlotType SlotType 
        => this.StorySubId == 0 ? GameSlotType.User : GameSlotType.Story;
    
    public bool IsLocked { get; set; }
    public bool IsSubLevel { get; set; }
    public bool IsCopyable { get; set; }
    public bool RequiresMoveController { get; set; }

    public GameLevel Clone()
    {
        return (GameLevel)this.MemberwiseClone();
    }
}