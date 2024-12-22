using Realms;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallengeScore : IRealmObject
{
    public GameChallenge Challenge { get; set; }

    /// <summary>
    /// The user submitting this score.
    /// </summary>
    public GameUser Publisher { get; set; }
    public long Score { get; set; }

    /// <summary>
    /// The hash referring to the ghost data for this score.
    /// </summary>
    public string GhostDataHash { get; set; } = "";

    public DateTimeOffset PublishDate { get; set; }
}