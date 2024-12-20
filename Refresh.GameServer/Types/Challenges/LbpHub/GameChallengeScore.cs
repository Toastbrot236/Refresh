using Realms;
using Refresh.Common.Constants;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallengeScore : IRealmObject  // Also used for ghost stuff
{
    /// <summary>
    /// idk lo
    /// </summary>
    public string Ghost { get; set; }  // Leaving this for experimenting for now
    public GameUser Publisher { get; set; }
    public GameChallenge Challenge { get; set; }
    public long Score { get; set; }

    /// <summary>
    /// The score players are supposed to compete against
    /// </summary>
    public bool OriginalScore { get; set; }
}