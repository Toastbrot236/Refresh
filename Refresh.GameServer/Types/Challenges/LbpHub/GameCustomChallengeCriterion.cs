using Realms;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameCustomChallengeCriterion : IRealmObject
{
    public GameCustomChallengeType Type
    {
        get => (GameCustomChallengeType)this._Type;
        set => this._Type = (byte)Value;
    }
    public byte _Type { get; set; }
    public long Value;
    public GameCustomChallenge Challenge { get; set; }
}