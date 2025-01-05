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
    public SerializedChallenge? UploadChallenge(RequestContext context, DataContext dataContext, GameUser user, SerializedChallenge body)
    {
        GameLevel? level = dataContext.Database.GetLevelByIdAndType(body.Level.Type, body.Level.LevelId);
        if (level == null) return null;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Challenge publisher: {body.PublisherName}");
        foreach (SerializedChallengeCriterion criterion in body.Criteria)
        {
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Challenge criterion type: {criterion.Type}, value: {criterion.Value}");
        }

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
            challenges = dataContext.Database.GetChallengesNotByUser(user, status);

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
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Score: {body.Score}");

        GameChallenge? challenge = dataContext.Database.GetChallengeById(challengeId);
        if (challenge == null) return NotFound;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Start CP: {challenge.StartCheckpointUid}");
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"End CP: {challenge.EndCheckpointUid}");

        bool isFirstScore = dataContext.Database.GetFirstScoreForChallenge(challenge) == null;
        GameAsset? ghostAsset = body.GhostHash == null ? null : dataContext.Database.GetAssetFromHash(body.GhostHash);

        // If there is no valid ghost data sent with the challenge score, reject the score.
        if (ghostAsset == null || ghostAsset.AssetType != GameAssetType.ChallengeGhost)
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

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"before GetGhostAssetContent");

        // Get the content of this score's ghost asset as a string
        string? ghostBodyString = this.GetGhostAssetContent(body, dataContext);
        
        // There is no way to catch all kinds of corruptions possible by LBP hub, neither is there a reliable way to correct corrupt ghost data either, 
        // so just try to catch a few easy and fortunately more common cases and reject the score if any of those cases are true.
        SerializedChallengeGhost? serializedGhost = this.IsGhostDataValid(ghostBodyString, isFirstScore, challenge, dataContext);
        if (serializedGhost == null)
        {
            dataContext.Database.AddErrorNotification(
                "Challenge Score upload failed", 
                $"Your score submission for challenge '{challenge.Name}' in level '{challenge.Level.Title} "
                +"could not be uploaded because your score's ghost data was corrupt."
                +"This happens sometimes. Try to submit another score!",
                user
            );
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost data not valid");
            return BadRequest;
        }

        // Count the ghost frames
        int ghostFramesCount = serializedGhost.Frames.Count;
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"ghost frames: {ghostFramesCount}");
        
        // Create score
        dataContext.Database.CreateChallengeScore(body, challenge, user, ghostFramesCount);
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"score created");

        // return the newly created score (not body)
        GameChallengeScore? newScore = dataContext.Database.GetHighScoreForChallengeByUser(challenge, user);
        return new Response(SerializedChallengeScore.FromOld(newScore), ContentType.Xml);
    }

    private string? GetGhostAssetContent(SerializedChallengeAttempt attempt, DataContext dataContext)
    {
        // At this point we already know the hash sent in the SerializedChallengeAttempt refers to an existing ghost asset
        if (!dataContext.DataStore.TryGetDataFromStore(attempt.GhostHash!, out byte[]? ghostDataBytes) || ghostDataBytes == null)
            return null;

        // Convert the contents of the ghost data into a string
        string ghostBodyString = Encoding.ASCII.GetString(ghostDataBytes, 0, ghostDataBytes.Length);
        
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset content (GetGhostAssetContent): {ghostBodyString}");
        return ghostBodyString;
    }

    private SerializedChallengeGhost? IsGhostDataValid(string? ghostBodyString, bool isFirstScore, GameChallenge challenge, DataContext dataContext)
    {
        if (ghostBodyString == null)
            return null;

        // Remove all duplicate "id" XML attributes in "metric" XML elements manually so we could deserialize with XmlSerializer
        string[] metricSubStrings = ghostBodyString.Split("<metric");
        string newGhostBodyString = metricSubStrings[0];

        // Start from second substring, since the first one is the one before the opening tag for the first occuring metric XML element
        for(int i = 1; i < metricSubStrings.Length; i++)
        {
            string substring = metricSubStrings[i];
            string[] idSubStrings = substring.Split(" id=");

            // usually all "id" XML attributes are set to the same value, so just take the value of the last attribute
            newGhostBodyString += "<metric id=" + idSubStrings.Last();
        }

        // Deserialize
        SerializedChallengeGhost? serializedGhost = null;
        try
        {
            XmlSerializer ghostSerializer = new(typeof(SerializedChallengeGhost));
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"ghostSerializer initialized");
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Ghost asset content (IsGhostDataValid): ");
            if (ghostSerializer.Deserialize(new StringReader(newGhostBodyString)) is not SerializedChallengeGhost data)
            {
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"ghostData not ghost");
                return null;
            }
            serializedGhost ??= data;
        }
        catch
        {
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"ghostData caught");
            return null;
        }

        if (serializedGhost == null)
        {
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"serializedGhost not set");
            return null;
        }

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"serializedGhost deserialized and set");

        if (serializedGhost.Checkpoints.Count < 1)
            return null;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Es ist checkpoints im haus, checkpoints: {serializedGhost.Checkpoints.Count}");

        // Normally the game already takes care of ordering the checkpoints by time, but to make sure order them ourselves aswell
        IEnumerable<SerializedChallengeCheckpoint> checkpoints = serializedGhost.Checkpoints.OrderBy(c => c.Time);
        if (checkpoints.First().Uid != challenge.StartCheckpointUid || checkpoints.Last().Uid != challenge.EndCheckpointUid)
            return null;

        // The end checkpoint cant appear more than once in a score which isnt the first score
        if (isFirstScore && checkpoints.Count(c => c.Uid == challenge.EndCheckpointUid) > 1)
            return null;

        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"first and last cp are correct, ghost data valid");
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

        // if requesting user is not in this list, fake the score in the response
        bool fakeScore = false;
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Fake score: {fakeScore}");

        // If user hasnt cleared the challenge yet
        if (dataContext.Database.GetClearsOfChallengeByUser(challenge, user) < 1)
        {
            dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Challenge not yet cleared");

            // No need to look for first score if user has cleared the challenge anyway
            GameChallengeScore? firstScore = dataContext.Database.GetFirstScoreForChallenge(challenge);

            // If there is no first score for this challenge, there are no scores at all. Return null.
            if (firstScore == null)
            {
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"No scores for callenge at all lol, return null");
                return null;
            }

            // If the request is for the same user who uploaded the first score, return it.
            if (firstScore.Publisher.UserId == requestedUser.UserId)
            {
                dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Returning first score");
                return SerializedChallengeScore.FromOld(dataContext.Database.GetFirstScoreForChallenge(challenge), 1, true);
            }
        }

        // Either the user has cleared the challenge atleast once or the request is not for the one who uploaded the first score,
        // either way return the high score of the requested user.
        dataContext.Logger.LogDebug(BunkumCategory.UserContent, $"Returning next high score");
        return SerializedChallengeScore.FromOld(dataContext.Database.GetHighScoreForChallengeByUser(challenge, user), 1, false);
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
        // Ignore username, return scores by the requesting user's friends
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

        IEnumerable<GameChallengeScoreWithRank> rankedScores = dataContext.Database.GetRankedHighScoresAroundChallengeScore(newestScore, 3);
        return new SerializedChallengeScoreList(SerializedChallengeScore.FromOldList(rankedScores));
    } 

    /// <summary>
    /// This endpoint gets called when clearing a challenge and seeing the leaderboard. It's supposed to return the score next to and behind
    /// the score you just submitted with SubmitChallengeScore for the leaderboard excerpt. I decided to refactor this endpoint's code into
    /// the GetContextualScoresForChallenge endpoint above, calling it with the user belonging to the username from the route parameters.
    /// </summary>
    [GameEndpoint("challenge/{challengeId}/scoreboard/{username}/contextual", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(NotFound)]
    public SerializedChallengeScoreList? GetContextualScoresByUserForChallenge(RequestContext context, DataContext dataContext, GameUser user, int challengeId, string username) 
    {
        GameUser? requestedUser = dataContext.Database.GetUserByUsername(username);
        if (requestedUser == null) return null;

        return this.GetContextualScoresForChallenge(context, dataContext, requestedUser, challengeId);
    } 

    #endregion

    #region Story Challenges

    // developer-challenges/scores?ids=1&ids=2&ids=3&ids=4
    [GameEndpoint("developer-challenges/scores", HttpMethods.Get, ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(OK)]
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