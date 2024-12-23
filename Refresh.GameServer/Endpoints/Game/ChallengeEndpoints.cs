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

    [GameEndpoint("user/{username}/friends/challenges", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetChallengesByUsersMutuals(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        /*
        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallengesByUser(user, status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
        */
        // lbp hub crashes when the list here and the one containing the users own challenge are the same,
        // return an empty list for now

        return new SerializedChallengeList();
    }

    #endregion

    #region Challenge Scores

    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, SerializedChallengeAttempt body, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        bool isOriginalScore = dataContext.Database.GetOriginalScoreForChallenge(challenge) == null;
        bool hasGhostData = dataContext.Database.GetAssetFromHash(body.GhostDataHash) != null;
        bool byChallengePublisher = challenge.Publisher.UserId == user.UserId;

        if (isOriginalScore)
        {
            if (byChallengePublisher)
            {
                if (!hasGhostData)
                {
                    // cannot prove validity of original score (which people are supposed to beat),
                    // therefore delete the challenge
                    dataContext.Database.RemoveChallenge(challenge);
                    dataContext.Database.AddErrorNotification(
                        "Challenge upload failed", 
                        $"Your challenge '{challenge.Name}' in level '{challenge.Level.Title}' could not be uploaded "
                        +"because the ghost data for its original score was missing.",
                        user
                    );
                    dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"No ghost data for original score by publisher");
                    return BadRequest;
                }

                else
                {
                    // Fallthrough, set original score
                }
            }

            else  // not by Challenge Publisher
            {
                // Reject, if someone tries to submit a score for a challenge which does not have an
                // original score, something has gone wrong. 
                // This case should normally not happen to begin with due to the case right above.
                // Existence of ghost data does not matter in this case.
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your latest score for challenge '{challenge.Name}' in level '{challenge.Level.Title}' "
                    +"could not be uploaded because there is no original score set for this challenge. "
                    +"This is not normal and should be reported to the challenge publisher.",
                    user
                );
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Original score, but not by challenge publisher");
                return BadRequest;
            }
        }

        else  // not the Original Score, but a submission
        {
            if (!hasGhostData)
            {
                // Cannot prove validity of this score, reject the score.
                // Publisher of score does not matter in this case.
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your recent score submission for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"could not be uploaded because your score's ghost data was missing.",
                    user
                );
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Score submission without ghost data");
                return BadRequest;
            }

            else
            {
                // Fallthrough, score submission
            }
        }

        // Create score
        dataContext.Database.CreateChallengeScore(body, challenge, user, isOriginalScore);
        return OK;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresForChallenge(RequestContext context, DataContext dataContext, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUser(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        return null;
    }

    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresForChallengeByUsersMutuals(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        // Take the mutuals of the user who made the request instead of the user specified in the route parameters
        // to not expose mutual data to other people
        return null;
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
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetContextualScoresForChallengeByUser(RequestContext context, DataContext dataContext, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) return null;

        return null;
    }

    [GameEndpoint("developer-challenges/scores")]
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

    #endregion
}