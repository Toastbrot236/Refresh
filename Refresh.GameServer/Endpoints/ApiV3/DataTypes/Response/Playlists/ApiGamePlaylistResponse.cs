using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Data;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Users;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Playlists;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Playlists;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGamePlaylistResponse : IApiResponse, IDataConvertableFrom<ApiGamePlaylistResponse, GamePlaylist>
{
    public required int PlaylistId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconHash { get; set; }
    public required ApiGameUserResponse? Publisher { get; set; }
    public required ApiGameLocationResponse Location { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset LastUpdateDate { get; set; }
    public required int Hearts { get; set; }

    public static ApiGamePlaylistResponse? FromOld(GamePlaylist? playlist, DataContext dataContext)
    {
        if (playlist == null) return null;

        return new ApiGamePlaylistResponse
        {
            PlaylistId = playlist.PlaylistId,
            Name = playlist.Name,
            Description = playlist.Description,
            IconHash = playlist.IconHash,
            Publisher = ApiGameUserResponse.FromOld(playlist.Publisher, dataContext),
            Location = ApiGameLocationResponse.FromLocation(playlist.LocationX, playlist.LocationY)!,
            CreationDate = playlist.CreationDate,
            LastUpdateDate = playlist.LastUpdateDate,
            Hearts = dataContext.Database.GetFavouriteCountForPlaylist(playlist),
        };
    }

    public static IEnumerable<ApiGamePlaylistResponse> FromOldList(IEnumerable<GamePlaylist> oldList, DataContext dataContext)
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}