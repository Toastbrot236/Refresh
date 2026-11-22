using Refresh.Core.Types.Data;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Levels;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameScoreResponse : IApiResponse, IDataConvertableFrom<ApiGameScoreResponse, GameScore>, IDataConvertableFrom<ApiGameScoreResponse, ScoreWithRank>
{
    public required string ScoreId { get; set; }
    public required ApiGameLevelResponse Level { get; set; }
    public required IEnumerable<ApiGameUserResponse> Players { get; set; } // TODO remove in APIv4, rename AllPlayers to just Players
    public required IEnumerable<ApiGameScorePlayerResponse> AllPlayers { get; set; }
    public required ApiMinimalUserResponse Publisher { get; set; }
    public required DateTimeOffset ScoreSubmitted { get; set; }
    public required int Score { get; set; }
    public required int Rank { get; set; }
    public required byte ScoreType { get; set; }
    
    public required TokenGame Game { get; set; }
    public required TokenPlatform Platform { get; set; }
    
    public static ApiGameScoreResponse? FromOld(GameScore? old, DataContext dataContext)
    {
        if (old == null) return null;

        IEnumerable<ApiGameScorePlayerResponse> players = ApiGameScorePlayerResponse.FromOldList(old.Players, dataContext);

        return new ApiGameScoreResponse
        {
            ScoreId = old.ScoreId.ToString()!,
            Level = ApiGameLevelResponse.FromOld(old.Level, dataContext)!,
            AllPlayers = players,
            Players = players.Where(p => p.User != null).Select(p => p.User!), // reuse to avoid duplicate DB lookups
            Publisher = ApiMinimalUserResponse.FromOld(old.Publisher, dataContext)!,
            ScoreSubmitted = old.ScoreSubmitted,
            Score = old.Score,
            ScoreType = old.ScoreType,
            Rank = old.Rank,
            Game = old.Game,
            Platform = old.Platform,
        };
    }

    public static ApiGameScoreResponse? FromOld(ScoreWithRank? old, DataContext dataContext)
        => FromOld(old?.score, dataContext);

    public static IEnumerable<ApiGameScoreResponse> FromOldList(IEnumerable<GameScore> oldList, DataContext dataContext) 
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
    
    public static IEnumerable<ApiGameScoreResponse> FromOldList(IEnumerable<ScoreWithRank> oldList, DataContext dataContext) 
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}