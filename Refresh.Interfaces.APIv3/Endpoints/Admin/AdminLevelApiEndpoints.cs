using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using MongoDB.Bson;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Levels;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminLevelApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/levels/id/{id}/teamPick", HttpMethods.Post), MinimumRole(GameUserRole.Curator)]
    [DocSummary("Marks a level as team picked.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    public ApiOkResponse AddTeamPickToLevel(RequestContext context, GameDatabaseContext database, GameUser user, int id)
    {
        GameLevel? level = database.GetLevelById(id); 
        if (level == null) return ApiNotFoundError.LevelMissingError;
        
        database.AddTeamPickToLevel(level);
        database.CreateEvent(level, new()
        {
            EventType = EventType.LevelTeamPick,
            Actor = user,
            OverType = EventOverType.Activity,
        });
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/levels/id/{id}/removeTeamPick", HttpMethods.Post), MinimumRole(GameUserRole.Curator)]
    [DocSummary("Removes a level's team pick status.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    public ApiOkResponse RemoveTeamPickFromLevel(RequestContext context, GameDatabaseContext database, GameUser user, int id)
    {
        GameLevel? level = database.GetLevelById(id);
        if (level == null) return ApiNotFoundError.LevelMissingError;
        
        database.RemoveTeamPickFromLevel(level);
        database.CreateEvent(level, new()
        {
            EventType = EventType.LevelUnTeamPick,
            Actor = user,
            // This should rarely happen. If it does, it should be public knowledge.
            // Also consistent behaviour with the endpoint above, even if this won't
            // be shown in-game, unlike team picking
            OverType = EventOverType.Activity,
        });
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/levels/id/{id}", HttpMethods.Patch), MinimumRole(GameUserRole.Curator)]
    [DocSummary("Updates a level.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), ApiAuthenticationError.NoPermissionsForObjectWhen)]
    public ApiResponse<ApiGameLevelResponse> EditLevelById(RequestContext context, GameDatabaseContext database,
        GameUser user,
        [DocSummary("The ID of the level")] int id, ApiAdminEditLevelRequest body, DataContext dataContext)
    {
        GameLevel? level = database.GetLevelById(id);
        if (level == null) return ApiNotFoundError.LevelMissingError;

        if (body.IconHash != null && body.IconHash.StartsWith('g') &&
            !dataContext.GuidChecker.IsTextureGuid(level.GameVersion, long.Parse(body.IconHash)))
            return ApiValidationError.InvalidTextureGuidError;
        
        level = database.UpdateLevel(body, level, user)!;
        database.CreateEvent(level, new()
        {
            EventType = EventType.LevelUpload,
            IsModified = true,
            Actor = user,
            OverType = EventOverType.Moderation,
            AdditionalInfo = $"Your level '{level.Title}' for game {level.GameVersion} has been updated by moderation"
        });

        return ApiGameLevelResponse.FromOld(level, dataContext);
    }
    
    [ApiV3Endpoint("admin/levels/id/{id}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes a level.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    public ApiOkResponse DeleteLevel(RequestContext context, GameDatabaseContext database, GameUser user, int id)
    {
        GameLevel? level = database.GetLevelById(id);
        if (level == null) return ApiNotFoundError.LevelMissingError;
        
        database.DeleteLevel(level);

        // We can just store the level ID here thanks to GameLevelRevisions documenting level revisions
        database.CreateEvent(level, new()
        {
            EventType = EventType.LevelUnpublish,
            Actor = user,
            AdditionalInfo = $"Your level '{level.Title}' for game {level.GameVersion} has been deleted by moderation",
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/levels/id/{id}/setAuthor", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Changes the author of a level. The new author must be an existing user on the server..")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.ObjectIdParseErrorWhen)]
    public ApiOkResponse SetLevelAuthor(RequestContext context, GameDatabaseContext database, GameUser user, int id, ApiSetLevelAuthorRequest body)
    {
        if (!ObjectId.TryParse(body.AuthorId, out ObjectId authorId))
            return ApiValidationError.ObjectIdParseError;
        
        GameLevel? level = database.GetLevelById(id);
        if (level == null)
            return ApiNotFoundError.LevelMissingError;
        
        GameUser? newAuthor = database.GetUserByObjectId(authorId);
        if (newAuthor == null)
            return ApiNotFoundError.UserMissingError;

        // TODO: Should this be logged using an event? Who would be the involved user? the old or the new user?
        
        database.UpdateLevelPublisher(level, newAuthor);
        return new ApiOkResponse();
    }
}