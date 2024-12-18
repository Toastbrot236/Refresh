using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

public partial class GameChallengeGhostFrame : IRealmObject
{
    public long Time { get; set; }
    public int LocationX { get; set; }
    public int LocationY { get; set; }
    public int LocationZ { get; set; }
    public float Rotation { get; set; }
    public bool Keyframe { get; set; }
    public GameChallengeScore Score { get; set; }
}