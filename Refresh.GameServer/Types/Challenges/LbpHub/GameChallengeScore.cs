using Realms;
using Refresh.GameServer.Database;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallengeScore : IRealmObject, ISequentialId
{
    [PrimaryKey] public int ScoreId { get; set; }
    public GameChallenge Challenge { get; set; }

    /// <summary>
    /// The user submitting this score.
    /// </summary>
    public GameUser Publisher { get; set; }

    /// <summary>
    /// The publisher's achieved raw score. More always means better here, independent of challenge type.
    /// </summary>
    public long Score { get; set; }

    /// <summary>
    /// The hash referring to the ghost data for this score. Gets set to null if this score is not the first score of
    /// its challenge and if it is not the high score of its publisher either.
    /// </summary>
    public string? GhostHash { get; set; }

    /// <summary>
    /// The number of ghost frame elements in this score's ghost asset.
    /// Useful for getting this score's time for challenges which are not time challenges.
    /// </summary>
    public int GhostFramesCount { get; set; }
    public DateTimeOffset PublishDate { get; set; }

    public int SequentialId
    {
        get => this.ScoreId;
        set => this.ScoreId = value;
    }
}