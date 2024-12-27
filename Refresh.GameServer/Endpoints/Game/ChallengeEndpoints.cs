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

    /// <summary>
    /// This endpoint is intended to return challenges by the specified user.
    /// Usually gets called together with the GetChallengesByUsersFriends endpoint below.
    /// The query parameter "status" indicates whether to return "active" or "expired" challenges.
    /// </summary>
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

    /// <summary>
    /// This endpoint is intended to get challenges by the specified user's friends. 
    /// Since probably not that many people will play LBP hub (considering its higher barrier to entry),
    /// it makes more sense to just return all challenges through this endpoint instead,
    /// excluding the challenges by the specified user (if found by the username in the route parameters) to prevent showing duplicates, 
    /// since this endpoint usually gets called together with the GetChallengesByUser endpoint above.
    /// The query parameter "status" indicates whether to return "active" or "expired" challenges.
    /// </summary>
    [GameEndpoint("user/{username}/friends/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetChallengesByUsersFriends(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);

        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges;
        if (user == null)
            challenges = dataContext.Database.GetChallenges(status);
        else
            challenges = dataContext.Database.GetNotUsersChallenges(user, status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    // This endpoint was probably intended to get both the user's and their friend's past challenges at once, as it only gets called
    // when you go to the pod menu's Past Challenges page (even though the game also sends the "status" query parameter).
    // Return all past challenges instead, similar reason as with the GetChallengesByUsersMutuals endpoint implementation above.
    [GameEndpoint("user/{username}/challenges/joined", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetJoinedChallenges(RequestContext context, DataContext dataContext, string username)
    {
        // ignore username

        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallenges(status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    #endregion

    #region Challenge Scores

    /// <summary>
    /// Gets called when submitting a challenge score after either clearing a challenge or right after uploading a challenge.
    /// Usually this endpoint only gets called after the game is done uploading the ChallengeGhost for this score and if UploadChallenge
    /// succeeds for the latter case. Therefore, if the ChallengeGhost referenced in the body can't be found in the data store, reject the score.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, SerializedChallengeAttempt body, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        bool isFirstScore = dataContext.Database.GetFirstScoreForChallenge(challenge) == null;
        bool noGhost = body.GhostHash == null || dataContext.Database.GetAssetFromHash(body.GhostHash) == null;

        // If there is no valid ghost data sent with the challenge score, reject the score.
        if (noGhost)
        {
            // If this is the first score of the challenge and uploaded by the challenge publisher (usually alongside the challenge itself),
            // also tell them about the state of the first score, otherwise only tell them why their score was rejected.
            if (isFirstScore && challenge.Publisher.UserId == user.UserId)
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score submission for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"could not be uploaded because your score's ghost data was missing. "
                    +"Whoever uploads a valid score first will have it set as the first score to beat",
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

    /// <summary>
    /// This endpoint returns the scores of a challenge. It usually gets called when viewing the leaderboard of a challenge in the pod menu
    /// and the game takes care of assigning rank numbers to scores.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/", HttpMethods.Get, ContentType.Xml)]  // Gets called in a level when playing a challenge
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]  // Gets called in the pod menu when viewing a challenge
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallenge(RequestContext context, DataContext dataContext, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallenge(challenge);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    /// <summary>
    /// This endpoint returns the scores of a challenge by a user. When you initiate a challenge against someone's ghost, the game calls
    /// this method for both your username and the username of the challenger you are going against. Probably intended to only return a single
    /// score with a rank assigned to it by the server.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUser(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallenge(challenge);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    /// <summary>
    /// This endpoint returns the scores of a challenge by a user's friends, specified by that user's username.
    /// We will just return the scores of the friends by the user calling this endpoint instead for privacy reasons.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUsersFriends(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        // Ignore username, return scores by the 

        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallengeByUsersMutuals(challenge, user);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    /// <summary>
    /// This endpoint gets called when clearing a challenge and seeing the leaderboard, but it doesnt seem like it actually
    /// achieves anything in-game. I decided to refactor the code from GetContextualScoresByUserForChallenge below into this method to
    /// have both do the same.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard//contextual" /*typo is intentional*/, HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetContextualScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameChallengeScore? newestScore = dataContext.Database.GetNewestScoreForChallengeByUser(challenge, user);
        if (newestScore == null) return null;

        return new SerializedChallengeScoreList(dataContext.Database.GetScoresAroundChallengeScore(newestScore, 3));
    } 

    /// <summary>
    /// This endpoint gets called when clearing a challenge and seeing the leaderboard. It's supposed to return the score next to and behind
    /// the score you just submitted with SubmitChallengeScore for the leaderboard excerpt. I decided to refactor this endpoint's code into
    /// the GetContextualScoresForChallenge endpoint above, calling it with the user belonging to the username from the route parameters.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/contextual", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetContextualScoresByUserForChallenge(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        return this.GetContextualScoresForChallenge(context, dataContext, user, challengeId);
    } 

    #endregion

    #region Developer Challenge Scores

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
    public Response SubmitDeveloperChallengeScore(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string body)
    {
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"SubmitDeveloperChallengeScore body: {body}");
        return NotImplemented;
    }

    #endregion
}