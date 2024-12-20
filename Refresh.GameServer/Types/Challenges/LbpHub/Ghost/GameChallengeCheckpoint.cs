using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

public partial class GameChallengeCheckpoint : IRealmObject
{
    public int Uid { get; set; }
    public long Time { get; set; }
    public GameChallengeScore Score { get; set; }

    /// <summary>
    /// Whether this is the start checkpoint of a challenge score
    /// </summary>
    public bool start { get; set; }

    /// <summary>
    /// Whether this is the finish checkpoint of a challenge score
    /// </summary>
    public bool finish { get; set; }
}