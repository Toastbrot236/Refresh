using MongoDB.Bson;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Levels.Scores;

#nullable disable

[Index(nameof(Game), nameof(Score), nameof(ScoreType))]
public partial class GameScore
{
    [Key] public ObjectId ScoreId { get; set; } = ObjectId.GenerateNewId();

    public TokenGame Game { get; set; }
    public TokenPlatform Platform { get; set; }

    [Required] public int LevelId { get; set; }
    [Required] public GameLevel Level { get; set; }
    public DateTimeOffset ScoreSubmitted { get; set; }
    
    public int Score { get; set; }
    public byte ScoreType { get; set; }
    
    public List<string> PlayerIdsRaw { get; set; } = [];
    [NotMapped] public List<ObjectId> PlayerIds => PlayerIdsRaw.Select(ObjectId.Parse).ToList();
    // set => PlayerIdsRaw = value.Select(v => v.ToString()).ToList();

    /// <summary>
    /// The actual publisher of this particular score.
    /// </summary>
    [ForeignKey(nameof(PublisherId)), Required] public GameUser Publisher { get; set; }
    [Required] public ObjectId PublisherId { get; set; }

    /// <summary>
    /// This score's cached rank on the global (non-friends) leaderboard.
    /// Null if this isn't a personal highscore and therefore not on the leaderboard.
    /// </summary>
    public int? Rank { get; set; }
}