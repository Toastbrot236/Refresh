using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.GameServer.Types.Assets;
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
    [RequireEmailVerified]
    [NullStatusCode(NotFound)]
    public Response UploadChallenge(RequestContext context, DataContext dataContext, GameUser user, SerializedChallenge body)
    {
        GameLevel? level = dataContext.Database.GetLevelByIdAndType(body.Level.Type, body.Level.LevelId);
        if (level == null) 
            return NotFound;
        
        if (body.Criteria.Count < 1) 
        {
            dataContext.Logger.LogWarning(BunkumCategory.UserContent, $"Challenge does not have any criteria, rejecting");
            return BadRequest;
        }

        if (body.Criteria.First().Value != 0)
            dataContext.Logger.LogWarning(BunkumCategory.UserContent, $"Challenge's criterion value is not 0, but {body.Criteria.First().Value}. This is an unknown case, the value won't be saved.");

        if (body.Criteria.Count > 1)
            dataContext.Logger.LogWarning(BunkumCategory.UserContent, $"Challenge has {body.Criteria.Count} criteria. This is an unknown case, only the first criterion's type will be saved.");

        GameChallenge challenge = dataContext.Database.CreateChallenge(body, level, user);

        // Return a SerializedChallenge which is not body, else the game will not send the first score
        // and it's ghost asset for this challenge
        return new Response(SerializedChallenge.FromOld(challenge, dataContext), ContentType.Xml);
    }

    /// <summary>
    /// Intended to return challenges by the specified user.
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
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallengesByUser(user, status)
            .OrderByDescending(c => c.ExpirationDate);
        
        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    /// <summary>
    /// Intended to return challenges by the specified user's friends,
    /// but since not that many people play LBP hub (considering its higher barrier to entry),
    /// it makes more sense to just return all challenges for this endpoint instead.
    /// Exclude challenges by the specified user (if found by the username in the route parameters) to not show duplicates in-game, 
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
            challenges = dataContext.Database.GetChallenges(status)
                .OrderByDescending(c => c.ExpirationDate);
        else
            challenges = dataContext.Database.GetChallengesNotByUser(user, status)
                .OrderByDescending(c => c.ExpirationDate);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    /// <summary>
    /// Most likely intended to get the specified user's and their friend's challenges.
    /// Return all challenges instead, for the same reason described in GetChallengesByUsersFriends' summary. 
    /// Usually this endpoint only gets called when going to "Past Challenges" in the pod.
    /// The query parameter "status" indicates whether to return "active" or "expired" challenges.
    /// </summary>
    [GameEndpoint("user/{username}/challenges/joined", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetJoinedChallenges(RequestContext context, DataContext dataContext, string username)
    {
        // ignore username, since we're returning all challenges with the specified status for this

        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallenges(status)
            .OrderByDescending(c => c.ExpirationDate);;

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    #endregion

    #region Challenge Scores

    /// <summary>
    /// Gets called when submitting a challenge score after either beating an opponent's challenge score or right after uploading a challenge.
    /// Usually this endpoint only gets called after the game is done uploading the ChallengeGhost asset for this score.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, SerializedChallengeAttempt body, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        bool isFirstScore = dataContext.Database.GetFirstScoreForChallenge(challenge) == null;
        GameAsset? ghostAsset = body.GhostHash == null ? null : dataContext.Database.GetAssetFromHash(body.GhostHash);

        // If there is no GameAsset in the database with the score's GhostHash, or the referred asset is not a ChallengeGhost for some reason,
        // reject the score.
        if (ghostAsset == null || ghostAsset.AssetType != GameAssetType.ChallengeGhost)
        {
            // If this is the first score of the challenge and uploaded by the challenge publisher (usually alongside the challenge itself),
            // also tell them about the state of the challenge's first score, otherwise only tell them why their score was rejected.
            if (isFirstScore)
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title}' "
                    +"couldn't be uploaded because it's ghost data was missing. "
                    +"Whoever uploads a valid score first will have it set as the first score to beat.",
                    user
                );
            }
            else
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title}' "
                    +"couldn't be uploaded because it's ghost data was missing.",
                    user
                );
            }

            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset with hash {body.GhostHash} was not found or is not a ChallengeGhost");
            return BadRequest;
        }

        SerializedChallengeGhost? serializedGhost = SerializedChallengeGhost.GetSerializedChallengeGhostFromDataStore(body.GhostHash, dataContext.DataStore);
        
        // If the ghost asset for this score is invalid, reject the score
        if (!SerializedChallengeGhost.IsGhostDataValid(serializedGhost, challenge, isFirstScore))
        {
            dataContext.Database.AddErrorNotification(
                "Challenge Score upload failed", 
                $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title}' "
                +"couldn't be uploaded because it's ghost data was corrupt. "
                +"Try to submit another score!",
                user
            );
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset with hash {body.GhostHash} is invalid");
            return BadRequest;
        }

        dataContext.Database.CreateChallengeScore(body, challenge, user, serializedGhost!.Frames.Count);
        return OK;
    }

    /// <summary>
    /// This endpoint returns the scores of a challenge. Normally the game takes care of assigning rank numbers to scores.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/", HttpMethods.Get, ContentType.Xml)]  // Called in a level when playing a challenge
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]  // Called in the pod menu when viewing a challenge
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        // Get and return the high scores (plus first score) of the challenge
        IEnumerable<GameChallengeScoreWithRank> scores = dataContext.Database.GetRankedScoresForChallenge(challenge);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores).ToList());
    }

    /// <summary>
    /// Intended to return the high score of a user for a challenge. Return the challenge's first score if
    /// the player hasn't cleared this challenge yet, otherwise the requested user's high score.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScore? GetUsersHighScoreForChallenge(RequestContext context, DataContext dataContext, GameUser requestingUser, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? requestedUser = dataContext.Database.GetUserByUsername(username);
        if (requestedUser == null) return null;

        // If there is no first score for this challenge, there are no scores at all.
        GameChallengeScoreWithRank? firstScore = dataContext.Database.GetRankedFirstScoreForChallenge(challenge);
        if (firstScore == null) return null;

        // If the requesting user hasnt cleared the challenge yet and the requested user is the first score's uploader,
        // return the first score, else the requested user's high score
        if (dataContext.Database.GetTotalChallengeClearsByUser(challenge, requestingUser) < 1 && firstScore.score.Publisher.UserId == requestedUser.UserId)
            return SerializedChallengeScore.FromOld(firstScore);
        else
            return SerializedChallengeScore.FromOld(dataContext.Database.GetRankedHighScoreByUserForChallenge(challenge, requestedUser));
    }

    /// <summary>
    /// Intended to return the scores of a challenge by a user's friends, specified by that user's username.
    /// Return the scores by the requesting user's friends instead for privacy reasons.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresByUsersFriendsForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScoreWithRank> rankedScores = dataContext.Database.GetRankedScoresByUsersMutualsForChallenge(challenge, user);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(rankedScores).ToList());
    }

    /// <summary>
    /// Gets called together with the other GetContextualScoresForChallenge endpoint below, but it doesn't actually do anything in-game.
    /// Stubbed to always return OK.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard//contextual" /*typo is intentional*/, HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response GetContextualScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId) 
        => OK;

    /// <summary>
    /// Gets called when a user finishes a challenge to show a 3 scores large fragment of it's leaderboard with the user's highscore preferrably being in the middle.
    /// Unlike in most other leaderboards, the game actually shows the score's ranks returned here.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/contextual", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetContextualScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username) 
    {
        GameUser? requestedUser = dataContext.Database.GetUserByUsername(username);
        if (requestedUser == null) return null;

        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameChallengeScoreWithRank? newestScore = dataContext.Database.GetRankedHighScoreByUserForChallenge(challenge, user);
        if (newestScore == null) return null;

        IEnumerable<GameChallengeScoreWithRank> rankedScores = dataContext.Database.GetRankedScoresAroundChallengeScore(newestScore.score, 3);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(rankedScores));
    } 

    #endregion

    #region Story Challenges
    // TODO: Implement story challenges

    // developer-challenges/scores?ids=1&ids=2&ids=3&ids=4
    [GameEndpoint("developer-challenges/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response GetDeveloperChallengeScores(RequestContext context, DataContext dataContext)
        => NotImplemented;

    // developer-challenges/3/scores
    [GameEndpoint("developer-challenges/{challengeId}/scores", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response SubmitDeveloperChallengeScore(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string body)
        => NotImplemented;

    #endregion
}