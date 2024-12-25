using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
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
    #region Challenges

    [GameEndpoint("challenge", HttpMethods.Post, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallenge? UploadChallenge(RequestContext context, DataContext dataContext, GameUser user, SerializedChallenge body)
    {
        GameLevel? level = dataContext.Database.GetLevelByIdAndType(body.Level.Type, body.Level.LevelId);
        if (level == null) return null;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Challenge publisher: {body.PublisherName}");

        GameChallenge challenge = dataContext.Database.CreateChallenge(body, level, user);

        // We have to return a SerializedChallenge, which is not body, else the game will not send the original score
        // and the ghost data belonging to it for this challenge
        return SerializedChallenge.FromOld(challenge, dataContext);
    }

    // Above endpoint gets called when you go to past challenges, idk whats different about it
    [GameEndpoint("user/{username}/challenges/joined", HttpMethods.Get, ContentType.Xml)]
    [GameEndpoint("user/{username}/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetChallengesByUser(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallengesByUser(user, status);
        
        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    // There doesn't seem to be an endpoint for just getting all challenges. When you go to the Player Challenges page in the pod menu for example,
    // the game will only get challenges from this endpoint and the one above (only yours and your mutuals). Especially considering LBP hub's
    // current circumstances, it makes way more sense to expose all challenges through this endpoint instead of just your mutuals challenges.
    [GameEndpoint("user/{username}/friends/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetChallengesByUsersMutuals(RequestContext context, DataContext dataContext, string username)
    {
        // ignore username
        
        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallenges(status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    #endregion

    #region Challenge Scores

    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, SerializedChallengeAttempt body, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        bool isFirstScore = dataContext.Database.GetFirstScoreForChallenge(challenge) == null;
        bool noGhostData = dataContext.Database.GetAssetFromHash(body.GhostDataHash) == null;

        // If there is no valid ghost data sent with the challenge score, reject the score.
        if (noGhostData)
        {
            // If this is the first score of the challenge and uploaded by the challenge publisher (usually alongside the challenge itself),
            // also tell them about the state of the first score, otherwise only tell them why their score was rejected.
            if (isFirstScore && challenge.Publisher.UserId == user.UserId)
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score submission for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"could not be uploaded because your score's ghost data was missing. "
                    +"Whoever uploads a valid score first will have it set as the score to beat",
                    user
                );
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"No ghost data for original score by publisher");
            }
            else
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score submission for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"could not be uploaded because your score's ghost data was missing.",
                    user
                );
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Score submission without ghost data");
            }

            return BadRequest;
        }

        // Create score
        dataContext.Database.CreateChallengeScore(body, challenge, user);
        return OK;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/", HttpMethods.Get, ContentType.Xml)]  // Gets called in a level when playing a challenge
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]  // Gets called in the pod menu when viewing a challenge
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallenge(RequestContext context, DataContext dataContext, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallenge(challenge, false);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUser(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallengeByUser(challenge, user, false);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUsersMutuals(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallengeByUsersMutuals(challenge, user, false);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard//contextual" /*typo is intentional*/, HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetContextualScoresForChallenge(RequestContext context, DataContext dataContext, int challengeId) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;
        
        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/contextual", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetContextualScoresByUserForChallenge(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        return null;
    }

    // developer-challenges/scores?ids=1&ids=2&ids=3&ids=4
    [GameEndpoint("developer-challenges/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetDeveloperChallengeScores(RequestContext context, DataContext dataContext)
    {
        string[]? developerChallengeIds = context.QueryString.GetValues("ids");
        if (developerChallengeIds == null) return null;

        List<GameLevel> levels = [];
        /*
        foreach (string developerChallengeIdStr in developerChallengeIds)
        {
            if (!int.TryParse(developerChallengeIdStr, out int developerChallengeId)) return null;
            GameLevel? level = dataContext.Database.GetLevelById(developerChallengeId);

            if (level == null) continue;
            
            levels.Add(level);
        }
        */

        return null;
    }

    // developer-challenges/3/scores
    [GameEndpoint("developer-challenges/{challengeId}/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitDeveloperChallengeScore(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        return NotImplemented;
    }

    #endregion
}