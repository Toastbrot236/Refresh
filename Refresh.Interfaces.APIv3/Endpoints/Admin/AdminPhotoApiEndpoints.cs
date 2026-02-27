using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Database;
using Refresh.Database.Models.Moderation;
using Refresh.Database.Models.Photos;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Documentation.Descriptions;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminPhotoApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/users/{idType}/{id}/photos", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes all photos posted by a user. Gets user by their UUID or username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiOkResponse DeletePhotosPostedByUser(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? targetUser = database.GetUserByIdAndType(idType, id);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;
        
        database.DeletePhotosPostedByUser(targetUser);
        database.CreateModerationAction(targetUser, ModerationActionType.PhotosByUserDeletion, user, ""); // TODO: Ability to include reason
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
        database.CreateModerationAction(photo, ModerationActionType.PhotoDeletion, user, ""); // TODO: Ability to include reason
        return new ApiOkResponse();
    }
}