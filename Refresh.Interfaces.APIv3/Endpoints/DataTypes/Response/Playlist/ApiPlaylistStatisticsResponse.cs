using Refresh.Core.Types.Data;
using Refresh.Database.Models.Statistics;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Playlists;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiPlaylistStatisticsResponse : IApiResponse, IDataConvertableFrom<ApiPlaylistStatisticsResponse, GamePlaylistStatistics>
{
    public required int Levels { get; set; }
    public required int SubPlaylists { get; set; }
    public required int ContainingPlaylists { get; set; }
    public required int Hearts { get; set; }

    public static ApiPlaylistStatisticsResponse? FromOld(GamePlaylistStatistics? old, DataContext dataContext)
    {
        if (old == null) return null;
        
        return new()
        {
            Levels = old.LevelCount,
            SubPlaylists = old.SubPlaylistCount,
            ContainingPlaylists = old.ParentPlaylistCount,
            Hearts = old.FavouriteCount,
        };
    }
    
    public static IEnumerable<ApiPlaylistStatisticsResponse> FromOldList(IEnumerable<GamePlaylistStatistics> oldList, DataContext dataContext)
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}