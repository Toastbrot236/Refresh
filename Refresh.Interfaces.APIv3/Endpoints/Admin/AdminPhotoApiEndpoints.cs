using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Moderation;
using Refresh.Database.Models.Photos;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminPhotoApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/users/uuid/{uuid}/photos", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all photos posted by a user. Gets user by their UUID.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeletePhotosPostedByUuid(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The UUID of the user")] string uuid)
    {
        GameUser? moderatedUser = database.GetUserByUuid(uuid);
        if (moderatedUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeletePhotosPostedByUser(moderatedUser);
        database.CreateModerationAction(moderatedUser, ModerationActionType.PhotosByUserDeletion, user, "-");
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/name/{username}/photos", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all photos posted by a user. Gets user by their username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeletePhotosPostedByUsername(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary("The username of the user")] string username)
    {
        GameUser? moderatedUser = database.GetUserByUsername(username);
        if (moderatedUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeletePhotosPostedByUser(moderatedUser);
        database.CreateModerationAction(moderatedUser, ModerationActionType.PhotosByUserDeletion, user, "-");
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/photos/id/{id}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes a photo.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PhotoMissingErrorWhen)]
    public ApiOkResponse DeletePhoto(RequestContext context, GameDatabaseContext database, GameUser user, int id)
    {
        GamePhoto? photo = database.GetPhotoById(id);
        if (photo == null) return ApiNotFoundError.PhotoMissingError;
        
        database.RemovePhoto(photo);
        database.CreateModerationAction(photo, ModerationActionType.PhotoDeletion, user, "-");
        return new ApiOkResponse();
    }
}