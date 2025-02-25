using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Database;
using Refresh.GameServer.Documentation.Attributes;
using Refresh.GameServer.Endpoints.ApiV3.ApiTypes;
using Refresh.GameServer.Endpoints.ApiV3.ApiTypes.Errors;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Request;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Levels;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Playlists;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Extensions;
using Refresh.GameServer.Types.Categories;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.Playlists;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Endpoints.ApiV3;

public class PlaylistApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("playlists"), Authentication(false)]
    [ClientCacheResponse(86400 / 2)] // cache for half a day
    [DocSummary("Retrieves a list of categories you can use to search playlists")]
    [DocQueryParam("includePreviews", "If true, a single playlist will be added to each category representing a playlist from that category. False by default.")]
    [DocError(typeof(ApiValidationError), "The boolean 'includePreviews' could not be parsed by the server.")]
    public ApiListResponse<ApiPlaylistCategoryResponse> GetPlaylistCategories(RequestContext context, CategoryService categories, DataContext dataContext)
    {
        bool result = bool.TryParse(context.QueryString.Get("includePreviews") ?? "false", out bool includePreviews);
        if (!result) return ApiValidationError.BooleanParseError;

        IEnumerable<ApiPlaylistCategoryResponse> response;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (includePreviews) response = ApiPlaylistCategoryResponse.FromOldList(categories.PlaylistCategories, context, dataContext);
        else response = ApiPlaylistCategoryResponse.FromOldList(categories.PlaylistCategories, dataContext);
        
        return new ApiListResponse<ApiPlaylistCategoryResponse>(response);
    }

    [ApiV3Endpoint("playlists/{route}"), Authentication(false)]
    [DocSummary("Retrieves a list of playlists from a category")]
    [DocError(typeof(ApiNotFoundError), "The playlist category cannot be found")]
    [DocUsesPageData]
    [DocQueryParam("username", "If set, certain categories like 'hearted' or 'byUser' will return the levels of " + 
                               "the user with this username instead of your own. Optional.")]
    public ApiListResponse<ApiGamePlaylistResponse> GetPlaylists(RequestContext context, CategoryService categories, GameUser? user,
        [DocSummary("The name of the category you'd like to retrieve levels from. " +
                    "Make a request to /levels to see a list of available categories")]
        string route, DataContext dataContext)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return new ApiError("You didn't specify a route. " +
                                "You probably meant to use the `/playlists` endpoint and left a trailing slash in the URL.", NotFound);
        }
        
        (int skip, int count) = context.GetPageData();

        DatabaseList<GamePlaylist>? list = categories.PlaylistCategories
            .FirstOrDefault(c => c.ApiRoute.StartsWith(route))?
            .Fetch(context, skip, count, dataContext, new LevelFilterSettings(context, TokenGame.Website), user);

        if (list == null) return ApiNotFoundError.Instance;

        DatabaseList<ApiGamePlaylistResponse> playlists = DatabaseList<ApiGamePlaylistResponse>.FromOldList<ApiGamePlaylistResponse, GamePlaylist>(list, dataContext);
        return playlists;
    }

    [ApiV3Endpoint("playlists/id/{id}"), Authentication(false)]
    [DocSummary("Gets an individual playlist by it's numerical ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    public ApiResponse<ApiGamePlaylistResponse> GetPlaylistById(DataContext dataContext,
        [DocSummary("The ID of the playlist")] int id)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(id);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;
        
        return ApiGamePlaylistResponse.FromOld(playlist, dataContext);
    }

    [ApiV3Endpoint("playlists/create", HttpMethods.Patch)]
    [DocSummary("Creates a new playlist")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), ApiAuthenticationError.NoPermissionsForObjectWhen)]
    [DocQueryParam("parentPlaylistId", "The id of the playlist to add the new playlist to. Playlist will be added to your root playlist if unspecified.")]
    public ApiOkResponse CreatePlaylist(RequestContext context, DataContext dataContext, GameUser user, 
        ApiPlaylistRequest body)
    {
        // if the player has no root playlist yet, create a new one first
        if (user.RootPlaylist == null)
        {
            GamePlaylist rootPlaylist = GamePlaylist.ToGamePlaylist("My Playlists", null, user, true);
            dataContext.Database.CreatePlaylist(rootPlaylist);
            dataContext.Database.SetUserRootPlaylist(user, rootPlaylist);
        }

        // create the actual playlist and add it to the root playlist to have it show up in lbp1 too
        GamePlaylist playlist = dataContext.Database.CreatePlaylist(user, body, false);
        dataContext.Database.AddPlaylistToPlaylist(playlist, user.RootPlaylist!);

        return new ApiOkResponse();
    }

    [ApiV3Endpoint("playlists/id/{id}", HttpMethods.Patch)]
    [DocSummary("Edits a playlist by it's numerical ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), ApiAuthenticationError.NoPermissionsForObjectWhen)]
    public ApiResponse<ApiGamePlaylistResponse> EditPlaylistById(RequestContext context, DataContext dataContext,
        [DocSummary("The ID of the playlist")] int id, ApiPlaylistRequest body)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(id);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;
        
        if (playlist.Publisher?.UserId != dataContext.User!.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        // playlists don't have game versions, just check if its a valid guid texture in LBP1 since that is the only
        // game which shows playlist's icons
        if (body.IconHash != null && body.IconHash.StartsWith('g') &&
            !dataContext.GuidChecker.IsTextureGuid(TokenGame.LittleBigPlanet1, long.Parse(body.IconHash)))
            return ApiValidationError.InvalidTextureGuidError;
        
        dataContext.Database.UpdatePlaylist(playlist, body);
        
        playlist = dataContext.Database.GetPlaylistById(id);
        return ApiGamePlaylistResponse.FromOld(playlist, dataContext);
    }

    [ApiV3Endpoint("playlists/id/{id}", HttpMethods.Delete)]
    [DocSummary("Deletes a playlist by it's numerical ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), ApiAuthenticationError.NoPermissionsForObjectWhen)]
    public ApiOkResponse DeletePlaylistById(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist")] int id)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(id);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        if (playlist.Publisher.UserId != user.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        dataContext.Database.DeletePlaylist(playlist);

        return new ApiOkResponse();
    }

    [ApiV3Endpoint("playlists/id/{id}/levels"), Authentication(false)]
    [DocSummary("Gets the levels of an individual playlist by it's numerical ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    public ApiListResponse<ApiGameLevelResponse> GetLevelsInPlaylist(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist")] int id)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(id);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        (int skip, int count) = context.GetPageData();

        IEnumerable<GameLevel> levels = dataContext.Database.GetLevelsInPlaylist(playlist);
        return new DatabaseList<ApiGameLevelResponse>
        (
            ApiGameLevelResponse.FromOldList(levels, dataContext).Skip(skip).Take(count)
        );
    }

    [ApiV3Endpoint("playlists/id/{playlistId}/add/level/{levelId}", HttpMethods.Post), Authentication(false)]
    [DocSummary("Adds the level specified by it's ID to the playlist specified by it's ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), "You lack the permissions to add levels to the specified parent playlist")]
    public ApiOkResponse AddLevelToPlaylist(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist to add to")] int playlistId, 
        [DocSummary("The ID of the level to add")] int levelId)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        GameLevel? level = dataContext.Database.GetLevelById(levelId);
        if (level == null) return ApiNotFoundError.LevelMissingError;

        if (level.Publisher?.UserId != dataContext.User!.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        dataContext.Database.AddLevelToPlaylist(level, playlist);
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("playlists/id/{playlistId}/remove/level/{levelId}", HttpMethods.Delete), Authentication(false)]
    [DocSummary("Removes the level specified by it's ID from the playlist specified by it's ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), "You lack the permissions to remove levels from the specified parent playlist")]
    public ApiOkResponse RemoveLevelFromPlaylist(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist to remove from")] int playlistId, 
        [DocSummary("The ID of the level to remove")] int levelId)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        GameLevel? level = dataContext.Database.GetLevelById(levelId);
        if (level == null) return ApiNotFoundError.LevelMissingError;

        if (level.Publisher?.UserId != dataContext.User!.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        dataContext.Database.RemoveLevelFromPlaylist(level, playlist);
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("playlists/id/{playlistId}/add/playlist/{levelId}", HttpMethods.Post), Authentication(false)]
    [DocSummary("Adds a child playlist specified by it's ID to a parent playlist specified by it's ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), "You lack the permissions to add playlists to the specified parent playlist")]
    public ApiOkResponse AddPlaylistToPlaylist(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist to add to")] int playlistId, 
        [DocSummary("The ID of the playlist to add")] int levelId)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        GameLevel? level = dataContext.Database.GetLevelById(levelId);
        if (level == null) return ApiNotFoundError.LevelMissingError;

        if (level.Publisher?.UserId != dataContext.User!.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        dataContext.Database.AddLevelToPlaylist(level, playlist);
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("playlists/id/{playlistId}/remove/playlist/{levelId}", HttpMethods.Delete), Authentication(false)]
    [DocSummary("Removes the playlist specified by it's ID from the playlist specified by it's ID")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.PlaylistMissingErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.LevelMissingErrorWhen)]
    [DocError(typeof(ApiAuthenticationError), "You lack the permissions to remove playlists from the specified parent playlist")]
    public ApiOkResponse RemovePlaylistFromPlaylist(RequestContext context, DataContext dataContext, GameUser user,
        [DocSummary("The ID of the playlist to remove from")] int playlistId, 
        [DocSummary("The ID of the playlist to remove")] int levelId)
    {
        GamePlaylist? playlist = dataContext.Database.GetPlaylistById(playlistId);
        if (playlist == null) return ApiNotFoundError.PlaylistMissingError;

        GameLevel? level = dataContext.Database.GetLevelById(levelId);
        if (level == null) return ApiNotFoundError.LevelMissingError;

        if (level.Publisher?.UserId != dataContext.User!.UserId) 
            return ApiAuthenticationError.NoPermissionsForObject;

        dataContext.Database.RemoveLevelFromPlaylist(level, playlist);
        return new ApiOkResponse();
    }
}