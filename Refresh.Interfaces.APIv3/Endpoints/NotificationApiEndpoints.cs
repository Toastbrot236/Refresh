using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using MongoDB.Bson;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Notifications;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Documentation.Attributes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;
using Refresh.Interfaces.APIv3.Extensions;

namespace Refresh.Interfaces.APIv3.Endpoints;

public class NotificationApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("notifications"), MinimumRole(GameUserRole.Restricted)]
    [DocUsesPageData, DocSummary("Gets a list of notifications stored for the user")]
    public ApiListResponse<ApiGameNotificationResponse> GetNotifications(RequestContext context, GameUser user,
        GameDatabaseContext database, DataContext dataContext)
    {
        (int skip, int count) = context.GetPageData();
        DatabaseList<GameNotification> notifications = database.GetNotificationsByUser(user, count, skip);
        return DatabaseListExtensions.FromOldList<ApiGameNotificationResponse, GameNotification>(notifications, dataContext);
    }

    [ApiV3Endpoint("notifications/{uuid}"), MinimumRole(GameUserRole.Restricted)]
    [DocSummary("Gets a specific notification for a user")]
    [DocError(typeof(ApiValidationError), ApiValidationError.ObjectIdParseErrorWhen)]
    [DocError(typeof(ApiNotFoundError), "The notification cannot be found")]
    public ApiResponse<ApiGameNotificationResponse> GetNotificationByUuid(RequestContext context, GameUser user,
        GameDatabaseContext database,
        [DocSummary("The UUID of the notification")]
        string uuid, DataContext dataContext)
    {
        bool parsed = ObjectId.TryParse(uuid, out ObjectId objectId);
        if (!parsed) return ApiValidationError.ObjectIdParseError;

        GameNotification? notification = database.GetNotificationByUuid(user, objectId);
        if (notification == null) return ApiNotFoundError.Instance;
        
        return ApiGameNotificationResponse.FromOld(notification, dataContext);
    }
    
    [ApiV3Endpoint("notifications/{uuid}", HttpMethods.Delete), MinimumRole(GameUserRole.Restricted)]
    [DocSummary("Clears an individual notification for a user")]
    [DocError(typeof(ApiValidationError), ApiValidationError.ObjectIdParseErrorWhen)]
    [DocError(typeof(ApiNotFoundError), "The notification cannot be found")]
    public ApiOkResponse ClearNotificationByUuid(RequestContext context, GameUser user, GameDatabaseContext database,
        [DocSummary("The UUID of the notification")] string uuid)
    {
        bool parsed = ObjectId.TryParse(uuid, out ObjectId objectId);
        if (!parsed) return ApiValidationError.ObjectIdParseError;
        
        GameNotification? notification = database.GetNotificationByUuid(user, objectId);
        if (notification == null) return ApiNotFoundError.Instance;
        
        database.DeleteNotification(notification);
        return new ApiOkResponse();
    }
    
    [ApiV3Endpoint("notifications", HttpMethods.Delete), MinimumRole(GameUserRole.Restricted)]
    [DocSummary("Clears all notifications stored for the user")]
    public ApiOkResponse ClearAllNotifications(RequestContext context, GameUser user, GameDatabaseContext database)
    {
        database.DeleteNotificationsByUser(user);
        return new ApiOkResponse();
    }
}