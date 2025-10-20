using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminLeaderboardApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/scores/{uuid}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Removes a score by the score's UUID.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.ScoreMissingErrorWhen)]
    public ApiOkResponse DeleteScore(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The UUID of the score")] string uuid)
    {
        GameScore? score = database.GetScoreByUuid(uuid);
        if (score == null) return ApiNotFoundError.Instance;

        GameLevel level = score.Level;

        database.DeleteScore(score);
        database.CreateEvent(score, new()
        {
            EventType = EventType.DeleteScore,
            Actor = user,
            AdditionalInfo = $"Your score of {score.Score} in {score.ScoreType}-player-mode has been deleted from level '{level.Title}' (ID: {level.LevelId}).",
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/scores", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all scores set by a user. Gets user by their UUID.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeleteScoresSetByUuid(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The UUID of the user")] string uuid)
    {
        GameUser? targetUser = database.GetUserByUuid(uuid);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeleteScoresSetByUser(targetUser);
        database.CreateEvent(targetUser, new()
        {
            EventType = EventType.DeleteUserScores,
            Actor = user,
            AdditionalInfo = $"All your scores have been moderated", // TODO: Ability for staff to enter an actual reason for all these endpoints
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/name/{username}/scores", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all scores set by a user. Gets user by their username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeleteScoresSetByUsername(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The username of the user")] string username)
    {
        GameUser? targetUser = database.GetUserByUsername(username);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeleteScoresSetByUser(user);
        database.CreateEvent(targetUser, new()
        {
            EventType = EventType.DeleteUserScores,
            Actor = user,
            AdditionalInfo = $"All your scores have been moderated",
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
}