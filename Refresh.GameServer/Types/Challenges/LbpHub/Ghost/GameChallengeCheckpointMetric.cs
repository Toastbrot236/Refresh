using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

public partial class GameChallengeCheckpointMetric : IRealmObject
{
    public int Id { get; set; }
    public long Value { get; set; }
    public GameChallengeCheckpoint Checkpoint { get; set; }
}