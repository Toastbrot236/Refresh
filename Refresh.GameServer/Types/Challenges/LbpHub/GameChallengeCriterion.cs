using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallengeCriterion : IRealmObject
{
    public GameChallengeType Type
    {
        get => (GameChallengeType)this._Type;
        set => this._Type = (byte)value;
    }
    public byte _Type { get; set; }
    public long Value;
    public GameChallenge Challenge { get; set; }
}