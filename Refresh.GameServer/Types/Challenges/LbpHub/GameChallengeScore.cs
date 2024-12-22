using Realms;
using Refresh.Common.Constants;
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

    /// <summary>
    /// Whether this is the specified challenge's original score, which players are supposed to beat.
    /// </summary>
    public bool OriginalScore { get; set; }
}