using Realms;
using Refresh.Common.Constants;
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
    public long Score { get; set; }
    public int StartCheckpointId { get; set; }
    public int EndCheckpointId { get; set; }
    public DateTimeOffset PublishDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public int SequentialId
    {
        get => this.ChallengeId;
        set => this.ChallengeId = value;
    }
}