using Realms;
using Refresh.GameServer.Database;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallenge : IRealmObject, ISequentialId
{
    [PrimaryKey] public int ChallengeId { get; set; }
    
    public string Name { get; set; } = "Unnamed Challenge";
    public GameUser Publisher { get; set; }
    public GameLevel Level { get; set; }

    /// <summary>
    /// The Uid of the checkpoint this challenge starts on.
    /// </summary>
    public int StartCheckpointUid { get; set; }

    /// <summary>
    /// The Uid of the checkpoint this challenge finishes on.
    /// </summary>
    public int EndCheckpointUid { get; set; }

    /// <summary>
    /// The score people are supposed to beat.
    /// </summary>
    public GameChallengeScore OriginalScore { get; set; }
    public DateTimeOffset CreationDate { get; set; }  // Date sent by game
    public DateTimeOffset PublishDate { get; set; }  // Date when database method is executed
    public DateTimeOffset LastUpdateDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }

    public int SequentialId
    {
        get => this.ChallengeId;
        set => this.ChallengeId = value;
    }
}