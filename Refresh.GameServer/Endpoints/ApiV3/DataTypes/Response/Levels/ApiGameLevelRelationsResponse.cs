using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Levels;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameLevelRelationsResponse : IApiResponse
{
    public required bool IsHearted { get; set; }
    public required bool IsQueued { get; set; }
    public required int TotalPlays { get; set; }
    public required int PhotosTaken { get; set; }

    public static ApiGameLevelRelationsResponse? FromOld(GameLevel level, DataContext dataContext)
    {
        GameUser? user = dataContext.User;
        if (user == null) return null;

        return new ApiGameLevelRelationsResponse
        {
            IsHearted = dataContext.Database.IsLevelFavouritedByUser(level, user),
            IsQueued = dataContext.Database.IsLevelQueuedByUser(level, user),
            TotalPlays = dataContext.Database.GetTotalPlaysForLevelByUser(level, user),
            PhotosTaken = dataContext.Database.GetTotalPhotosInLevelByUser(level, user),
        };
    }
}