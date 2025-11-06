using Refresh.Database.Models.Levels;

namespace Refresh.Database.Models.Statistics;

/// <summary>
/// "Tracks" statistics for a particular leaderboard of a level, which are only seperated by score type for now. 
/// Currently only used to track when to recalculate the cached ranks of the global leaderboards' high scores,
/// which are saved in <see cref="Levels.Scores.GameScore"/>
/// </summary>
[PrimaryKey(nameof(LevelId), nameof(ScoreType))]
public class GameLevelLeaderboardStatistics
{
    [Required] public int LevelId { get; set; }
    [Required, ForeignKey(nameof(LevelId))] public GameLevel Level { get; set; } = null!;

    /// <summary>
    /// Further defines which leaderboard this object is for, exactly.
    /// </summary>
    public byte ScoreType { get; set; }
    public DateTimeOffset? RecalculateAt { get; set; } = null;
    public int Version { get; set; } = GameDatabaseContext.LevelLeaderboardStatisticsVersion;
}