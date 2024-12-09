using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Storage;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.GameServer.Database;
using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Lists;
using Refresh.GameServer.Types.Roles;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Endpoints.Game;

public class ChallengeEndpoints : EndpointGroup
{
    [GameEndpoint("user/{username}/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedCustomChallengeList? GetChallenges(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        IEnumerable<GameCustomChallenge> challenges = dataContext.Database.GetChallengesByUser(user);
        
        return new SerializedCustomChallengeList(
            SerializedCustomChallenge.FromOldList(challenges, dataContext)
        );
    }
}