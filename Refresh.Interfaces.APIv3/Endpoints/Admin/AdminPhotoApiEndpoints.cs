using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Activity;
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
    public ApiOkResponse DeletePhotosPostedByUuid(RequestContext context, GameDatabaseContext database,
        [DocSummary("The UUID of the user")] string uuid, GameUser user)
    {
        GameUser? targetUser = database.GetUserByUuid(uuid);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeletePhotosPostedByUser(targetUser);
        database.CreateEvent(targetUser, new()
        {
            EventType = EventType.DeleteUserPhotos,
            Actor = user,
            AdditionalInfo = $"All your photos have been deleted by moderation",
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/users/name/{username}/photos", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all photos posted by a user. Gets user by their username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeletePhotosPostedByUsername(RequestContext context, GameDatabaseContext database,
        [DocSummary("The username of the user")] string username, GameUser user)
    {
        GameUser? targetUser = database.GetUserByUsername(username);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeletePhotosPostedByUser(targetUser);
        database.CreateEvent(targetUser, new()
        {
            EventType = EventType.DeleteUserPhotos,
            Actor = user,
            AdditionalInfo = $"All your photos have been deleted by moderation",
            OverType = EventOverType.Moderation,
        });

        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("admin/photos/id/{id}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes a photo.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PhotoMissingErrorWhen)]
    public ApiOkResponse DeletePhoto(RequestContext context, GameDatabaseContext database, int id, GameUser user)
    {
        GamePhoto? photo = database.GetPhotoById(id);
        if (photo == null) return ApiNotFoundError.PhotoMissingError;
        
        database.RemovePhoto(photo);
        database.CreateEvent(photo, new()
        {
            EventType = EventType.DeletePhoto,
            Actor = user,
            // TODO: come up with a user friendly way to identify photos in text.
            // Level title and type? upload date?
            AdditionalInfo = $"Your photo has been deleted by moderation",
            OverType = EventOverType.Moderation,
        });
        return new ApiOkResponse();
    }
}