using Refresh.Core.Types.Data;
using Refresh.Database.Models.Playlists;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Playlists;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiOwnPlaylistStatisticsResponse
{
    public bool IsHearted { get; set; }

    public static ApiOwnPlaylistStatisticsResponse? FromOld(GamePlaylist? old, DataContext dataContext)
    {
        if (old == null || dataContext.User == null) return null;
        
        return new()
        {
            IsHearted = dataContext.Database.IsPlaylistFavouritedByUser(old, dataContext.User),
        };
    }
}