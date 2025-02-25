using Bunkum.Core;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Categories.Playlists;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Playlists;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Playlists;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiPlaylistCategoryResponse : IApiResponse, IDataConvertableFrom<ApiPlaylistCategoryResponse, GamePlaylistCategory>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconHash { get; set; }
    public required string FontAwesomeIcon { get; set; }
    public required string ApiRoute { get; set; }
    public required bool RequiresUser { get; set; }
    public required ApiGamePlaylistResponse? PreviewPlaylist { get; set; }
    public required bool Hidden { get; set; } = false;
    
    public static ApiPlaylistCategoryResponse? FromOld(GamePlaylistCategory? old, GamePlaylist? previewPlaylist,
        DataContext dataContext)
    {
        if (old == null) return null;

        return new ApiPlaylistCategoryResponse
        {
            Name = old.Name,
            Description = old.Description,
            IconHash = old.IconHash,
            FontAwesomeIcon = old.FontAwesomeIcon,
            ApiRoute = old.ApiRoute,
            RequiresUser = old.RequiresUser,
            PreviewPlaylist = ApiGamePlaylistResponse.FromOld(previewPlaylist, dataContext),
            Hidden = old.Hidden,
        };
    }
    
    public static ApiPlaylistCategoryResponse? FromOld(GamePlaylistCategory? old, DataContext dataContext) => FromOld(old, null, dataContext);

    public static IEnumerable<ApiPlaylistCategoryResponse> FromOldList(IEnumerable<GamePlaylistCategory> oldList,
        DataContext dataContext) => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
    
    public static IEnumerable<ApiPlaylistCategoryResponse> FromOldList(IEnumerable<GamePlaylistCategory> oldList,
        RequestContext context,
        DataContext dataContext)
    {
        return oldList.Select(category =>
        {
            DatabaseList<GamePlaylist>? list = category.Fetch(context, 0, 1, dataContext, new LevelFilterSettings(context, TokenGame.Website), dataContext.User);
            GamePlaylist? playlist = list?.Items.FirstOrDefault();
            
            return FromOld(category, playlist, dataContext);
        }).ToList()!;
    }
}