using Realms;
using Refresh.Common.Constants;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameChallengeScore : IRealmObject
{
    public string Ghost { get; set; } = SystemUsers.UnknownUserName;
    public GameUser Publisher { get; set; }
    public GameChallenge Challenge { get; set; }
    public long Score { get; set; }
}