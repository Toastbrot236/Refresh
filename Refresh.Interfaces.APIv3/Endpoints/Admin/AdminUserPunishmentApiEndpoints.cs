using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Moderation;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminUserPunishmentApiEndpoints : EndpointGroup
{
    private static ApiOkResponse BanUser(GameUser? target, GameUser moderator, GameDatabaseContext database, ApiPunishUserRequest body)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.BanUser(target, body.Reason, body.ExpiryDate);
        database.CreateModerationAction(target, ModerationActionType.UserPunishment, moderator, $"Banned until {body.ExpiryDate} for the following reason: {body.Reason}");
        return new ApiOkResponse();
    }
    
    private static ApiOkResponse RestrictUser(GameUser? target, GameUser moderator, GameDatabaseContext database, ApiPunishUserRequest body)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.RestrictUser(target, body.Reason, body.ExpiryDate);
        database.CreateModerationAction(target, ModerationActionType.UserPunishment, moderator, $"Restricted until {body.ExpiryDate} for the following reason: {body.Reason}");
        return new ApiOkResponse();
    }
    
    private static ApiOkResponse PardonUser(GameUser? target, GameUser moderator, GameDatabaseContext database)
    {
        if (target == null) return ApiNotFoundError.UserMissingError;
        
        database.SetUserRole(target, GameUserRole.User);
        database.CreateModerationAction(target, ModerationActionType.UserPunishment, moderator, "-");
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/users/name/{username}/ban", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Bans a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse BanByUsername(RequestContext context, GameDatabaseContext database, string username, GameUser user, ApiPunishUserRequest body) 
        => BanUser(database.GetUserByUsername(username), user, database, body);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/ban", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Bans a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse BanByUuid(RequestContext context, GameDatabaseContext database, string uuid, GameUser user, ApiPunishUserRequest body) 
        => BanUser(database.GetUserByUuid(uuid), user, database, body);
    
    [ApiV3Endpoint("admin/users/name/{username}/restrict", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Restricts a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse RestrictByUsername(RequestContext context, GameDatabaseContext database, string username, GameUser user, ApiPunishUserRequest body) 
        => RestrictUser(database.GetUserByUsername(username), user, database, body);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/restrict", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Restricts a user for the specified reason until the given date.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocRequestBody(typeof(ApiPunishUserRequest))]
    public ApiOkResponse RestrictByUuid(RequestContext context, GameDatabaseContext database, string uuid, GameUser user, ApiPunishUserRequest body) 
        => RestrictUser(database.GetUserByUuid(uuid), user, database, body);
    
    [ApiV3Endpoint("admin/users/name/{username}/pardon", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Pardons all punishments for the given user.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse PardonByUsername(RequestContext context, GameDatabaseContext database, GameUser user, string username) 
        => PardonUser(database.GetUserByUsername(username), user, database);
    
    [ApiV3Endpoint("admin/users/uuid/{uuid}/pardon", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Pardons all punishments for the given user.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse PardonByUuid(RequestContext context, GameDatabaseContext database, GameUser user, string uuid) 
        => PardonUser(database.GetUserByUuid(uuid), user, database);
}