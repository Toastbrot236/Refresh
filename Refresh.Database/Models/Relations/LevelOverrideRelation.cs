using MongoDB.Bson;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Reports;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Relations;

[PrimaryKey(nameof(LevelRootHash), nameof(UserId))]
public partial class LevelOverrideRelation
{
    // Level ID, incase the user has added this override from an existing level
    public int? LevelId { get; set; }

    [ForeignKey(nameof(LevelId))]
    public GameLevel? Level { get; set; }

    // Report ID, incase the override is either from a report's initial, or current level state
    public int? GriefReportId { get; set; }

    [ForeignKey(nameof(LevelId))]
    public GameGriefReport? GriefReport { get; set; }

    [Required]
    public ObjectId UserId { get; set; }

    [ForeignKey(nameof(UserId)), Required]
    public GameUser User { get; set; } = null!;

    // Every level (and therefore every override) has a root hash, not every override has a level ID,
    // so have this be part of PK instead
    [Required]
    public string LevelRootHash { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }
}