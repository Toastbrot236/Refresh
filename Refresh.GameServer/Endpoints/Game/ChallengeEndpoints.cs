using System.Text;
using System.Xml.Serialization;
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
    [MinimumRole(GameUserRole.Restricted)]
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
            dataContext.Logger.LogWarning(BunkumCategory.UserContent, $"Challenge has not one but {body.Criteria.Count} criteria. This is an unknown case, only the first criterion's type will be saved.");

        GameChallenge challenge = dataContext.Database.CreateChallenge(body, level, user);

        // We have to return a SerializedChallenge, which is not body, else the game will not send the first score
        // and the ghost data belonging to it for this challenge
        return new Response(SerializedChallenge.FromOld(challenge, dataContext), ContentType.Xml);
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
    /// This endpoint is intended to get challenges by the specified user's friends,
    /// but since probably not that many people will play LBP hub (considering its higher barrier to entry),
    /// it makes more sense to just return all challenges with the specified status for this endpoint instead,
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
            challenges = dataContext.Database.GetChallengesNotByUser(user, status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    /// <summary>
    /// This endpoint is most likely intended to get your and your friends challenges, but we will return all challenges
    /// with the specified status instead (see GetChallengesByUsersFriends' summary). Usually only gets called when going to past challenges
    /// in the pod.
    /// The query parameter "status" indicates whether to return "active" or "expired" challenges.
    /// </summary>
    [GameEndpoint("user/{username}/challenges/joined", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeList? GetJoinedChallenges(RequestContext context, DataContext dataContext, string username)
    {
        // ignore username, since we're returning all challenges with the specified status for this

        string? status = context.QueryString.Get("status");
        IEnumerable<GameChallenge> challenges = dataContext.Database.GetChallenges(status);

        return new SerializedChallengeList(SerializedChallenge.FromOldList(challenges, dataContext).ToList());
    }

    #endregion

    #region Challenge Scores

    /// <summary>
    /// Gets called when submitting a challenge score after either beating an opponents challenge score or right after uploading a challenge.
    /// Usually this endpoint only gets called after the game is done uploading the ChallengeGhost asset for this score.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Post, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitChallengeScore(RequestContext context, DataContext dataContext, GameUser user, SerializedChallengeAttempt body, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        bool isFirstScore = dataContext.Database.GetFirstScoreForChallenge(challenge) == null;
        GameAsset? ghostAsset = body.GhostHash == null ? null : dataContext.Database.GetAssetFromHash(body.GhostHash);

        // If there is no asset on the server corresponding to the score's GhostHash, or the referred asset is not a ChallengeGhost for some reason,
        // reject the score.
        if (ghostAsset == null || ghostAsset.AssetType != GameAssetType.ChallengeGhost)
        {
            // If this is the first score of the challenge and uploaded by the challenge publisher (usually alongside the challenge itself),
            // also tell them about the state of the challenge's first score, otherwise only tell them why their score was rejected.
            if (isFirstScore && challenge.Publisher.UserId == user.UserId)
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"couldn't be uploaded because it's ghost data was missing. "
                    +"Whoever uploads a valid score first will have it set as the first score to beat",
                    user
                );
            }
            else
            {
                dataContext.Database.AddErrorNotification(
                    "Challenge Score upload failed", 
                    $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                    +"couldn't be uploaded because it's ghost data was missing.",
                    user
                );
            }

            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset with hash {body.GhostHash} was not found or is not a ChallengeGhost");
            return BadRequest;
        }

        // Get the content of this score's ghost asset as a string
        string? ghostContentString = this.GetGhostAssetContent(body, dataContext);
        
        // There is no way to catch all kinds of corruptions possible by LBP hub, neither is there a reliable way to correct corrupt ghost data either, 
        // so just try to catch a few easy and fortunately more common cases and reject the score if any of those cases are true.
        SerializedChallengeGhost? serializedGhost = this.IsGhostDataValid(ghostContentString, isFirstScore, challenge, dataContext);
        if (serializedGhost == null)
        {
            dataContext.Database.AddErrorNotification(
                "Challenge Score upload failed", 
                $"Your score for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                +"couldn't be uploaded because it's ghost data was corrupt. "
                +"Try to submit another score!",
                user
            );
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset with hash {body.GhostHash} is invalid");
            return BadRequest;
        }

        // Create the score
        dataContext.Database.CreateChallengeScore(body, challenge, user, serializedGhost.Frames.Count);
        return OK;
    }

    private string? GetGhostAssetContent(SerializedChallengeAttempt attempt, DataContext dataContext)
    {
        // At this point we already know the hash sent in the SerializedChallengeAttempt refers to an existing ghost asset.
        // If reading from the ghost asset fails, return
        if (!dataContext.DataStore.TryGetDataFromStore(attempt.GhostHash, out byte[]? ghostContentBytes) || ghostContentBytes == null)
            return null;

        // Convert the contents of the ghost asset into a string
        return Encoding.ASCII.GetString(ghostContentBytes);
    }

    private SerializedChallengeGhost? IsGhostDataValid(string? ghostContentString, bool isFirstScore, GameChallenge challenge, DataContext dataContext)
    {
        if (ghostContentString == null)
            return null;

        // Remove all duplicate "id" XML attributes in "metric" XML elements manually to keep the XmlSerializer happy
        string[] metricSubstrings = ghostContentString.Split("<metric");
        string fixedGhostContentString = metricSubstrings[0];

        // Start iterating from second substring, since the first one is the one before the opening tag for the first occuring metric element
        for(int i = 1; i < metricSubstrings.Length; i++)
        {
            string substring = metricSubstrings[i];
            string[] idSubstrings = substring.Split(" id=");

            // Usually all "id" XML attributes are set to the same value, so just take the value of the last attribute.
            // Also we don't even need the metrics for validation.
            fixedGhostContentString += "<metric id=" + idSubstrings.Last();
        }

        // Try to deserialize the ghost asset
        SerializedChallengeGhost? serializedGhost = null;
        try
        {
            XmlSerializer ghostSerializer = new(typeof(SerializedChallengeGhost));
            if (ghostSerializer.Deserialize(new StringReader(fixedGhostContentString)) is not SerializedChallengeGhost output)
                return null;

            serializedGhost ??= output;
        }
        catch
        {
            return null;
        }

        // If serializedGhost is still null for some reason or there are no checkpoints at all in it, return
        if (serializedGhost == null || serializedGhost.Checkpoints.Count < 1)
            return null;

        // Normally the game already takes care of ordering the checkpoints by time, but just in case
        IEnumerable<SerializedChallengeCheckpoint> checkpoints = serializedGhost.Checkpoints.OrderBy(c => c.Time);

        // The first checkpoint must be the start checkpoint of the challenge and the last checkpoint must be the end checkpoint,
        // otherwise the score is not valid
        if (checkpoints.First().Uid != challenge.StartCheckpointUid || checkpoints.Last().Uid != challenge.EndCheckpointUid)
            return null;

        // The end checkpoint cant appear more than once in a score which is not the first score, 
        // because the game immediately ends the challenge you are playing when you reach it
        if (isFirstScore && checkpoints.Count(c => c.Uid == challenge.EndCheckpointUid) > 1)
            return null;

        // Checks successful, ghost is valid
        return serializedGhost;
    }

    /// <summary>
    /// This endpoint returns the scores of a challenge. It usually gets called when viewing the leaderboard of a challenge in the pod menu
    /// and the game takes care of assigning rank numbers to scores.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/", HttpMethods.Get, ContentType.Xml)]  // Gets called in a level when playing a challenge
    [GameEndpoint("challenge/{challengeId}/scoreboard", HttpMethods.Get, ContentType.Xml)]  // Gets called in the pod menu when viewing a challenge
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        // Get and return the high scores (plus first score) of the challenge
        IEnumerable<GameChallengeScore> scores = dataContext.Database.GetScoresForChallenge(challenge);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(scores, dataContext).ToList());
    }

    /// <summary>
    /// This endpoint is supposed to return the high score of a user for a challenge, but we will just return the first score if
    /// the player hasn't cleared this challenge yet, otherwise the next highest high score after the players high score. 
    /// given user's high score.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScore? GetUsersHighScoreForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username) 
    {
        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameUser? requestedUser = dataContext.Database.GetUserByUsername(username);
        if (requestedUser == null) return null;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"beginning");
        // If there is no first score for this challenge, there are no scores at all. Return NotFound.
        GameChallengeScore? firstScore = dataContext.Database.GetFirstScoreForChallenge(challenge);
        if (firstScore == null) return null;
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"first score found");

        // If the requesting user hasnt cleared the challenge yet and the requested user is the first score's uploader,
        // return the first score, else the requested user's high score
        if (dataContext.Database.GetTotalChallengeClearsByUser(challenge, user) < 1 && firstScore.Publisher.UserId == requestedUser.UserId)
            return SerializedChallengeScore.FromOld(dataContext.Database.GetRankedFirstScoreForChallenge(challenge));
        else
            return SerializedChallengeScore.FromOld(dataContext.Database.GetRankedHighScoreForChallengeByUser(challenge, requestedUser));
    }

    /// <summary>
    /// This endpoint returns the scores of a challenge by a user's friends, specified by that user's username.
    /// We will just return the scores of the friends by the user calling this endpoint instead for privacy reasons.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/friends", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetScoresByUsersFriendsForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId)
    {
        // Ignore username, return scores by the requesting user's friends

        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        IEnumerable<GameChallengeScoreWithRank> rankedScores = dataContext.Database.GetRankedScoresForChallengeByUsersMutuals(challenge, user);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(rankedScores).ToList());
    }

    /// <summary>
    /// This endpoint gets called together with the GetContextualScoresByUserForChallenge endpoint below when finishing a challenge
    /// and seeing your place in the leaderboard, but it doesn't seem like this endpoint's response actually affects anything.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard//contextual" /*typo is intentional*/, HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public Response GetContextualScoresForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId) 
        => OK;

    /// <summary>
    /// This endpoint gets called together with the GetContextualScoresByUserForChallenge endpoint below when finishing a challenge
    /// and seeing your place in the leaderboard. Unlike in most other leaderboards, the game actually shows the score's ranks returned
    /// here. The game expects 3 scores here, with yours being in the middle. 
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/contextual", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetContextualScoresByUserForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username) 
    {
        GameUser? requestedUser = dataContext.Database.GetUserByUsername(username);
        if (requestedUser == null) return null;

        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return null;

        GameChallengeScore? newestScore = dataContext.Database.GetNewestScoreForChallengeByUser(challenge, user);
        if (newestScore == null) return null;

        IEnumerable<GameChallengeScoreWithRank> rankedScores = dataContext.Database.GetRankedHighScoresAroundChallengeScore(newestScore, 3);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(rankedScores));
    } 

    #endregion

    #region Story Challenges

    // developer-challenges/scores?ids=1&ids=2&ids=3&ids=4
    [GameEndpoint("developer-challenges/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotImplemented)]
    public SerializedChallengeScoreList? GetDeveloperChallengeScores(RequestContext context, DataContext dataContext)
    {
        /*
        string[]? challengeIdStrings = context.QueryString.GetValues("ids");
        if (challengeIdStrings == null) return null;

        List<GameStoryChallenge> challenges = [];
        foreach (string challengeIdString in challengeIdStrings)
        {
            if (!int.TryParse(challengeIdString, out int challengeId)) continue;
            GameStoryChallenge challenge = dataContext.Database.GetStoryChallengeById(challengeId);

            if (challenge == null) continue;
            
            challenges.Add(challenge);
        }
        
        return challenges;
        */

        return null;
    }

    // developer-challenges/3/scores
    [GameEndpoint("developer-challenges/{challengeId}/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response SubmitDeveloperChallengeScore(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string body)
    {
        //dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"SubmitDeveloperChallengeScore body: {body}");
        return NotImplemented;
    }

    #endregion
}