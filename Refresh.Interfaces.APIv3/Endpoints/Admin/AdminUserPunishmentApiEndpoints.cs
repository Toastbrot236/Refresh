using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminUserPunishmentApiEndpoints : EndpointGroup
{
    private static ApiOkResponse BanUser(GameUser actor, GameUser? target, GameDatabaseContext database, ApiPunishUserRequest body)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.BanUser(target, body.Reason, body.ExpiryDate);
        database.CreateEvent(target, new()
        {
            EventType = EventType.BanUser,
            Actor = actor,
            AdditionalInfo = $"Banned for the following reason: {body.Reason}",
            OverType = EventOverType.Moderation,
        });
        return new ApiOkResponse();
    }
    
    private static ApiOkResponse RestrictUser(GameUser actor, GameUser? target, GameDatabaseContext database, ApiPunishUserRequest body)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.RestrictUser(target, body.Reason, body.ExpiryDate);
        database.CreateEvent(target, new()
        {
            EventType = EventType.RestrictUser,
            Actor = actor,
            AdditionalInfo = $"Restricted for the following reason: {body.Reason}",
            OverType = EventOverType.Moderation,
        });
        return new ApiOkResponse();
    }
    
    private static ApiOkResponse PardonUser(GameUser actor, GameUser? target, GameDatabaseContext database)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.SetUserRole(target, GameUserRole.User);
        database.CreateEvent(target, new()
        {
            EventType = EventType.PardonUser,
            Actor = actor,
            OverType = EventOverType.Moderation,
        });
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/users/name/{username}/ban", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Bans a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse BanByUsername(RequestContext context, GameDatabaseContext database, GameUser user, string username, ApiPunishUserRequest body) 
        => BanUser(user, database.GetUserByUsername(username), database, body);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/ban", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Bans a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse BanByUuid(RequestContext context, GameDatabaseContext database, GameUser user, string uuid, ApiPunishUserRequest body) 
        => BanUser(user, database.GetUserByUuid(uuid), database, body);
    
    [ApiV3Endpoint("admin/users/name/{username}/restrict", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Restricts a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse RestrictByUsername(RequestContext context, GameDatabaseContext database, GameUser user, string username, ApiPunishUserRequest body) 
        => RestrictUser(user, database.GetUserByUsername(username), database, body);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/restrict", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Restricts a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse RestrictByUuid(RequestContext context, GameDatabaseContext database, GameUser user, string uuid, ApiPunishUserRequest body) 
        => RestrictUser(user, database.GetUserByUuid(uuid), database, body);
    
    [ApiV3Endpoint("admin/users/name/{username}/pardon", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Pardons all punishments for the given user.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse PardonByUsername(RequestContext context, GameDatabaseContext database, GameUser user, string username) 
        => PardonUser(user, database.GetUserByUsername(username), database);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/pardon", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Pardons all punishments for the given user.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse PardonByUuid(RequestContext context, GameDatabaseContext database, GameUser user, string uuid) 
        => PardonUser(user, database.GetUserByUuid(uuid), database);
}