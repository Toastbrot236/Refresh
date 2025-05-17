using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.GameServer.Configuration;
using Refresh.Database;
using Refresh.GameServer.Endpoints.Game.DataTypes.Response;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Lists;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Playlists;
using Refresh.GameServer.Types.Playlists;

namespace Refresh.GameServer.Endpoints.Game.Playlists;

public class Lbp3PlaylistEndpoints : EndpointGroup 
{
    [GameEndpoint("playlists", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response CreatePlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, SerializedLbp3Playlist body)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        // if the player has no root playlist yet, create a new one first
        if (user.RootPlaylist == null)
        {
            dataContext.Database.CreateRootPlaylist(user);
        }

        // create the actual playlist and add it to the root playlist
        GamePlaylist playlist = dataContext.Database.CreatePlaylist(user, body);
        dataContext.Database.AddPlaylistToPlaylist(playlist, user.RootPlaylist!);

        // return the playlist we just created to have the game open to it immediately
        return new Response(SerializedLbp3Playlist.FromOld(playlist, dataContext), ContentType.Xml);
    }

    [GameEndpoint("playlists/{playlistId}", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response UpdatePlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, SerializedLbp3Playlist body, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        // don't allow the wrong user to update playlists
        if (playlist.Publisher.UserId != user.UserId)
            return Unauthorized;
        
        dataContext.Database.UpdatePlaylist(playlist, body);
        
        // get playlist from database a second time to respond with it in its updated state
        // to have it immediately update in-game
        GamePlaylist newPlaylist = dataContext.Database.GetPlaylistById(playlistId)!;
        return new Response(SerializedLbp3Playlist.FromOld(newPlaylist, dataContext), ContentType.Xml);
    }

    [GameEndpoint("playlists/{playlistId}/delete", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response DeletePlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        // don't allow the wrong user to delete playlists
        if (playlist.Publisher.UserId != user.UserId)
            return Unauthorized;

        dataContext.Database.DeletePlaylist(playlist);
        return OK;
    }

    [GameEndpoint("playlists/{playlistId}/slots", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotFound)]
    [MinimumRole(GameUserRole.Restricted)]
    public SerializedLevelList? GetPlaylistLevels(RequestContext context, DataContext dataContext, GameUser user, int playlistId)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null)
            return null;

        DatabaseList<GameLevel> levels = dataContext.Database.GetLevelsInPlaylist(playlist, dataContext.Game, 0, 100);

        return new SerializedLevelList
        {
            Items = GameLevelResponse.FromOldList(levels.Items, dataContext).ToList(),
            Total = levels.TotalItems,
            NextPageStart = levels.NextPageIndex,
        };
    }

    [GameEndpoint("playlists/{playlistId}/slots", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response AddLevelsToPlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, SerializedLevelIdList body, GameUser user, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        // Dont let people add levels to other's playlists
        if (playlist.Publisher.UserId != user.UserId) 
            return Unauthorized;

        dataContext.Database.AddLevelsToPlaylist(body.LevelIds, playlist);
        return OK;
    }

    [GameEndpoint("playlists/{playlistId}/order_slots", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response ReorderPlaylistLevels(RequestContext context, GameServerConfig config, DataContext dataContext, SerializedLevelIdList body, GameUser user, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null)
            return NotFound;

        // Dont let people reorder levels in other's playlists
        if (playlist.Publisher.UserId != user.UserId)
            return Unauthorized;

        dataContext.Database.ReorderLevelsInPlaylist(body.LevelIds, playlist);
        return OK;
    }

    [GameEndpoint("playlists/{playlistId}/slots/{levelId}/delete", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response RemoveLevelFromPlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, int playlistId, int levelId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        GameLevel? level = dataContext.Database.GetLevelById(levelId);
        if (level == null) 
            return NotFound;

        // Dont let people remove levels from other's playlists
        if (playlist.Publisher.UserId != user.UserId)
            return Unauthorized;

        dataContext.Database.RemoveLevelFromPlaylist(level, playlist);
        return OK;
    }

    [GameEndpoint("user/{username}/playlists", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotFound)]
    [MinimumRole(GameUserRole.Restricted)]
    public SerializedLbp3PlaylistList? GetPlaylistsByUser(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) 
            return null;

        // LBP3 only sets pagination parameters when actually viewing the playlists by a user (where it'll not paginate the playlists properly anyway), 
        // outside of that it'll use this endpoint, without specifying the pagination request parameters, over and over again just to get 3 level icons for a
        // preview. That is very inefficient, so just return 10 playlists when no parameters are specified.
        bool paginationParamsGiven = int.TryParse(context.QueryString["pageStart"], out int _) || int.TryParse(context.QueryString["pageSize"], out int _);
        (int skip, int count) = paginationParamsGiven ? (0, 100) : (0, 10);

        DatabaseList<GamePlaylist> playlists = dataContext.Database.GetPlaylistsByAuthor(user, skip, count);

        return new SerializedLbp3PlaylistList 
        {
            Items = SerializedLbp3Playlist.FromOldList(playlists.Items, dataContext).ToList(),
            Total = playlists.TotalItems,
            NextPageStart = playlists.NextPageIndex,
        };
    }

    [GameEndpoint("favouritePlaylists/{username}", HttpMethods.Get, ContentType.Xml)]
    [NullStatusCode(NotFound)]
    [MinimumRole(GameUserRole.Restricted)]
    public SerializedLbp3PlaylistList? GetFavouritedPlaylists(RequestContext context, DataContext dataContext, string username)
    {
        GameUser? user = dataContext.Database.GetUserByUsername(username);
        if (user == null) 
            return null;

        (int skip, int count) = context.GetPageData();
        DatabaseList<GamePlaylist> playlists = dataContext.Database.GetPlaylistsFavouritedByUser(user, skip, count);

        return new SerializedLbp3FavouritePlaylistList
        {
            Items = SerializedLbp3Playlist.FromOldList(playlists.Items, dataContext).ToList(),
            Total = playlists.TotalItems,
            NextPageStart = playlists.NextPageIndex,
        };
    }

    [GameEndpoint("favourite/playlist/{playlistId}", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response FavouritePlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        dataContext.Database.FavouritePlaylist(playlist, user);
        return OK;
    }

    [GameEndpoint("unfavourite/playlist/{playlistId}", HttpMethods.Post, ContentType.Xml)]
    [RequireEmailVerified]
    public Response UnfavouritePlaylist(RequestContext context, GameServerConfig config, DataContext dataContext, GameUser user, int playlistId)
    {
        if (user.IsWriteBlocked(config))
            return Unauthorized;

        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) 
            return NotFound;

        dataContext.Database.UnfavouritePlaylist(playlist, user);
        return OK;
    }
}