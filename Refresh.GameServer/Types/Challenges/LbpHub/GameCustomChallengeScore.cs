using Realms;
using Refresh.Common.Constants;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

public partial class GameCustomChallengeScore : IRealmObject
{
    public string Ghost { get; set; } = SystemUsers.UnknownUserName;
    public GameUser Player { get; set; }
    public GameCustomChallenge Challenge { get; set; }
    public long Score { get; set; }
}