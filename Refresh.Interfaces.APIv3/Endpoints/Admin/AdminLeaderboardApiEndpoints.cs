using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Moderation;
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
        
        database.DeleteScore(score);
        // TODO: Ability, or maybe even force moderators to specify an actual reason for every tracked action.
        database.CreateModerationAction(score, ModerationActionType.ScoreDeletion, user, "-");
        
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/scores", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all scores set by a user. Gets user by their UUID.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeleteScoresSetByUuid(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The UUID of the user")] string uuid)
    {
        GameUser? moderatedUser = database.GetUserByUuid(uuid);
        if (moderatedUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeleteScoresSetByUser(moderatedUser);
        database.CreateModerationAction(moderatedUser, ModerationActionType.ScoresByUserDeletion, user, "-");
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/name/{username}/scores", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all scores set by a user. Gets user by their username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeleteScoresSetByUsername(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The username of the user")] string username)
    {
        GameUser? moderatedUser = database.GetUserByUsername(username);
        if (moderatedUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeleteScoresSetByUser(moderatedUser);
        database.CreateModerationAction(moderatedUser, ModerationActionType.ScoresByUserDeletion, user, "-");
        return new ApiOkResponse();
    }
}