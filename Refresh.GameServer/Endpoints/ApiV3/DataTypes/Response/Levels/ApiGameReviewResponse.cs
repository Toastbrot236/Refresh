using Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Users;
using Refresh.GameServer.Types;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Reviews;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Levels;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameReviewResponse : IApiResponse, IDataConvertableFrom<ApiGameReviewResponse, GameReview>
{
    public required int ReviewId { get; set; }
    public required ApiGameLevelResponse Level { get; set; }
    public required ApiGameUserResponse Publisher { get; set; }
    public required DateTimeOffset PostedAt { get; set; }
    public required string Labels { get; set; }
    public required string Text { get; set; }
    public required int LevelRating { get; set; }

    public required int PositiveRating { get; set; }
    public required int NegativeRating { get; set; }
    public required int OwnReviewRating { get; set; }
    public static ApiGameReviewResponse? FromOld(GameReview? old, DataContext dataContext)
    {
        if (old == null) return null;

        DatabaseRating reviewRatings = dataContext.Database.GetRatingForReview(old);

        return new ApiGameReviewResponse
        {
            ReviewId = old.ReviewId,
            Level = ApiGameLevelResponse.FromOld(old.Level, dataContext)!,
            Publisher = ApiGameUserResponse.FromOld(old.Publisher, dataContext)!,
            PostedAt = old.PostedAt,
            Labels = old.Labels,
            Text = old.Content,
            LevelRating = dataContext.Database.GetLevelRatingByUser(old.Publisher, old.Level)?.ToDPad() ?? 0,
            PositiveRating = reviewRatings.PositiveRating,
            NegativeRating = reviewRatings.NegativeRating,
            OwnReviewRating = dataContext.Database.GetReviewRatingByUser(old.Publisher, old)?.ToDPad() ?? 0,
        };
    }

    public static IEnumerable<ApiGameReviewResponse> FromOldList(IEnumerable<GameReview> oldList,
        DataContext dataContext) => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}