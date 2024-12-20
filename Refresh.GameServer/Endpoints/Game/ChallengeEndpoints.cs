using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
using Bunkum.Core.Storage;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.Lists;
using Refresh.GameServer.Types.Roles;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Endpoints.Game;

public class ChallengeEndpoints : EndpointGroup
{
    [GameEndpoint("user/{username}/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetChallengesByUser(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallengesByUser(user);
        
        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    [GameEndpoint("challenge", HttpMethods.Post, ContentType.Xml)]
    [NullStatusCode(NotFound)]
    public SerializedChallenge? UploadChallenge(RequestContext context, DataContext dataContext, GameUser user, SerializedChallenge body)
    {
        dataContext.Logger.LogInfo(BunkumCategory.UserContent, $"Sent Challenge level: {body.Level.Type}, {body.Level.LevelId}, {body.Level.Title}");

        GameLevel? level = dataContext.Database.GetLevelByIdAndType(body.Level.Type, body.Level.LevelId);
        if (level == null) return null;

        dataContext.Logger.LogInfo(BunkumCategory.UserContent, $"Found Challenge level: {level.LevelId}, {level.Title}");

        GameChallenge challenge = dataContext.Database.CreateChallenge(body, level, user);

        return SerializedChallenge.FromOld(challenge, dataContext);
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetMutualsChallengeScores(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username)
    {
        // ignore username in route parameters, privacy lol
        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetOwnChallengeScores(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username) 
    {
        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard//contextual", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetContextualChallengeScores(RequestContext context, DataContext dataContext, GameUser user, int challengeId) 
    {
        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetChallengeScores(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [NullStatusCode(NotImplemented)]
    public Response? SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, string body, int challengeId)
    {
        dataContext.Logger.LogInfo(BunkumCategory.UserContent, $"Request body: {body}");
        return null;
    }

    [GameEndpoint("developer-challenges/scores")]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetDeveloperChallengeScores(RequestContext context, DataContext dataContext, GameUser user)
    {
        string[]? developerChallengeIds = context.QueryString.GetValues("ids");
        if (developerChallengeIds == null) return null;

        List<GameLevel> levels = [];
        
        foreach (string developerChallengeIdStr in developerChallengeIds)
        {
            if (!int.TryParse(developerChallengeIdStr, out int developerChallengeId)) return null;
            GameLevel? level = dataContext.Database.GetLevelById(developerChallengeId);

            if (level == null) continue;
            
            levels.Add(level);
        }

        return null;
    }
}