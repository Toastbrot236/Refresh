using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
using Bunkum.Core.Storage;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Configuration;
using Refresh.Core.Services;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Assets;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Photos;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.Game.Types.Lists;

namespace Refresh.Interfaces.Game.Endpoints;

public class PhotoEndpoints : EndpointGroup
{
    [GameEndpoint("uploadPhoto", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response UploadPhoto(RequestContext context, SerializedPhoto body, GameDatabaseContext database,
        GameUser user, IDataStore dataStore,
        DataContext dataContext, AipiService aipi, GameServerConfig config)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;
        
        if (!dataStore.ExistsInStore(body.SmallHash) ||
            !dataStore.ExistsInStore(body.MediumHash) ||
            !dataStore.ExistsInStore(body.LargeHash) ||
            !dataStore.ExistsInStore(body.PlanHash))
        {
            database.AddErrorNotification("Photo upload failed", "The required assets were not available.", user);
            return BadRequest;
        }

        if (body.PhotoSubjects.Count > 4)
        {
            context.Logger.LogWarning(BunkumCategory.UserContent, $"Too many subjects in photo, rejecting photo upload. Uploader: {user.UserId}");
            return BadRequest;
        }

        List<string> hashes = [body.LargeHash, body.MediumHash, body.SmallHash];
        foreach (string hash in hashes.Distinct())
        {
            GameAsset? gameAsset = database.GetAssetFromHash(hash);
            if(gameAsset == null) continue;
            if (aipi != null && aipi.ScanAndHandleAsset(dataContext, gameAsset))
                return Unauthorized;
        }

        database.UploadPhoto(body, user);

        return OK;
    }

    [GameEndpoint("deletePhoto/{id}", HttpMethods.Post)]
    public Response DeletePhoto(RequestContext context, GameDatabaseContext database, GameUser user, int id)
    {
        GamePhoto? photo = database.GetPhotoById(id);
        if (photo == null) return NotFound;

        if (photo.Publisher.UserId != user.UserId)
            return Unauthorized;
        
        database.RemovePhoto(photo);
        return OK;
    }
    
    private static Response GetPhotos(RequestContext context, GameDatabaseContext database, DataContext dataContext, Func<GameUser, int, int, DatabaseList<GamePhoto>> photoGetter)
    {
        string? username = context.QueryString.Get("user");
        if (username == null) return BadRequest;

        GameUser? user = database.GetUserByUsername(username);
        if (user == null) return NotFound;
        
        (int skip, int count) = context.GetPageData();

        // count not used ingame
        IEnumerable<SerializedPhoto> photos = photoGetter.Invoke(user, count, skip)
            .Items
            .ToArray()
            .Select(photo => PhotoExtensions.FromGamePhoto(photo, dataContext));

        return new Response(new SerializedPhotoList(photos), ContentType.Xml);
    }

    [GameEndpoint("photos/with", ContentType.Xml)]
    [Authentication(false)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response PhotosWithUser(RequestContext context, GameDatabaseContext database, DataContext dataContext) 
        => GetPhotos(context, database, dataContext, database.GetPhotosWithUser);
    
    [GameEndpoint("photos/by", ContentType.Xml)]
    [Authentication(false)]
    [MinimumRole(GameUserRole.Restricted)]
    public Response PhotosByUser(RequestContext context, GameDatabaseContext database, DataContext dataContext) 
        => GetPhotos(context, database, dataContext, database.GetPhotosByUser);

    [GameEndpoint("photos/{slotType}/{levelId}", ContentType.Xml)]
    public SerializedPhotoList? GetPhotosOnLevel(RequestContext context, DataContext dataContext, string slotType, int levelId)
    {
        GameLevel? level = dataContext.Database.GetLevelByIdAndType(slotType, levelId);
        if (level == null) 
            return null;

        (int skip, int count) = context.GetPageData();
        
        // if the game specifies whose photos to get which are uploaded for this level
        string? username = context.QueryString.Get("by");
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        
        DatabaseList<GamePhoto> photos;
        
        if (user != null)
            photos = dataContext.Database.GetPhotosInLevelByUser(level, user, count, skip);
        else
            photos = dataContext.Database.GetPhotosInLevel(level, count, skip);

        // count not used ingame
        return new SerializedPhotoList(photos.Items.ToArrayIfPostgres().Select(photo => PhotoExtensions.FromGamePhoto(photo, dataContext)));
    }

    [GameEndpoint("photo/{id}", ContentType.Xml)]
    [NullStatusCode(NotFound)]
    public SerializedPhoto? GetPhotoById(RequestContext context, DataContext dataContext, int id)
    {
        GamePhoto? photo = dataContext.Database.GetPhotoById(id);

        if (photo == null) 
            return null;
        
        return PhotoExtensions.FromGamePhoto(photo, dataContext);
    }
}