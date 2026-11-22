using Refresh.Core.Types.Data;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Levels;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameScorePlayerResponse : IApiResponse, IDataConvertableFrom<ApiGameScorePlayerResponse, GameScorePlayer>
{
    public required string ScoreId { get; set; }
    public required string Username { get; set; }
    public required ApiGameUserResponse? User { get; set; }
    // TODO no need to include player number yet, photo subjects don't include them either
    
    public static ApiGameScorePlayerResponse? FromOld(GameScorePlayer? old, DataContext dataContext)
    {
        if (old == null) return null;

        return new ApiGameScorePlayerResponse
        {
            ScoreId = old.ScoreId.ToString(),
            Username = old.Username,
            User = ApiGameUserResponse.FromOld(old.User, dataContext),
        };
    }
    
    public static IEnumerable<ApiGameScorePlayerResponse> FromOldList(IEnumerable<GameScorePlayer> oldList, DataContext dataContext) 
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}