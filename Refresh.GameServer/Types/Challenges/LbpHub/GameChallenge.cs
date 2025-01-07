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
    /// The Uid of the checkpoint this challenge ends on.
    /// </summary>
    public int EndCheckpointUid { get; set; }

    /// <summary>
    /// Whether this is a score/time/lives etc challenge.
    /// </summary>
    public GameChallengeType Type
    {
        get => (GameChallengeType)this._Type;
        set => this._Type = (byte)value;
    }
    public byte _Type { get; set; }

    public DateTimeOffset PublishDate { get; set; }

    // TODO: This could store the last time this challenge's metadata was edited through ApiV3 in the future,
    // such as changing the name or extending the expiration date. Right now this is only being set to the publish date.
    public DateTimeOffset LastUpdateDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }

    public int SequentialId
    {
        get => this.ChallengeId;
        set => this.ChallengeId = value;
    }
}