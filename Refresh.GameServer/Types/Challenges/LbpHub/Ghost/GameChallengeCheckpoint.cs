using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

public partial class GameChallengeCheckpoint : IRealmObject
{
    public int Uid { get; set; }
    public long Time { get; set; }
    public GameChallengeScore Score { get; set; }
}