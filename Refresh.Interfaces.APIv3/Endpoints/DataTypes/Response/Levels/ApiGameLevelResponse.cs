using System.Diagnostics;
using Refresh.Core.Types.Data;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Comments;
using Refresh.Database.Models.Levels;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Data;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Levels;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameLevelResponse : IApiResponse, IDataConvertableFrom<ApiGameLevelResponse, GameLevel>
{
    public required int LevelId { get; set; }
    public required ApiGameUserResponse? Publisher { get; set; }
    public required bool IsReUpload { get; set; }
    public required bool IsModded { get; set; }
    public required string? OriginalPublisher { get; set; }

    public required bool IsAdventure { get; set; }
    public required string Title { get; set; }
    public required string IconHash { get; set; }
    public required string Description { get; set; }
    public required ApiGameLocationResponse Location { get; set; }
    
    public required string RootLevelHash { get; set; }
    public TokenGame GameVersion { get; set; }
    
    public required DateTimeOffset PublishDate { get; set; }
    public required DateTimeOffset UpdateDate { get; set; }
    
    public required int MinPlayers { get; set; }
    public required int MaxPlayers { get; set; }
    public required bool EnforceMinMaxPlayers { get; set; }
    
    public required bool SameScreenGame { get; set; }
    
    public required IEnumerable<ApiGameSkillRewardResponse>? SkillRewards { get; set; }
    
    public required int YayRatings { get; set; }
    public required int BooRatings { get; set; }
    public required int Hearts { get; set; }
    public required int PhotosTaken { get; set; }
    public required int LevelComments { get; set; }
    public required int Reviews { get; set; }
    
    public required int UniquePlays { get; set; }
    public required bool TeamPicked { get; set; }
    public required DateTimeOffset? DateTeamPicked { get; set; }
    public required GameLevelType LevelType { get; set; }
    public required GameSlotType SlotType { get; set; }
    public required bool IsLocked { get; set; }
    public required bool IsSubLevel { get; set; }
    public required bool IsCopyable { get; set; }
    public required float Score { get; set; }
    public required IEnumerable<Tag> Tags { get; set; }

    public static ApiGameLevelResponse? FromOld(GameLevel? level, DataContext dataContext)
    {
        if (level == null) return null;
        
        if(level.Statistics == null)
            dataContext.Database.RecalculateLevelStatistics(level);
        
        Debug.Assert(level.Statistics != null);

        return new ApiGameLevelResponse
        {
            IsAdventure = level.IsAdventure,
            Title = level.Title,
            Publisher = ApiGameUserResponse.FromOld(level.Publisher, dataContext),
            OriginalPublisher = level.OriginalPublisher,
            IsReUpload = level.IsReUpload,
            LevelId = level.LevelId,
            IconHash = GetIconHash(level, dataContext),
            Description = level.Description,
            Location = ApiGameLocationResponse.FromLocation(level.LocationX, level.LocationY)!,
            PublishDate = level.PublishDate,
            UpdateDate = level.UpdateDate,
            MinPlayers = level.MinPlayers,
            MaxPlayers = level.MaxPlayers,
            EnforceMinMaxPlayers = level.EnforceMinMaxPlayers,
            SameScreenGame = level.SameScreenGame,
            SkillRewards = ApiGameSkillRewardResponse.FromOldList(dataContext.Database.GetSkillRewardsForLevel(level), dataContext),
            YayRatings = level.Statistics.YayCount,
            BooRatings = level.Statistics.BooCount,
            Hearts = level.Statistics.FavouriteCount,
            UniquePlays = level.Statistics.UniquePlayCount,
            TeamPicked = level.TeamPicked,
            DateTeamPicked = level.DateTeamPicked,
            RootLevelHash = level.RootResource,
            GameVersion = level.GameVersion,
            LevelType = level.LevelType,
            SlotType = level.SlotType,
            IsCopyable = level.IsCopyable,
            IsLocked = level.IsLocked,
            IsSubLevel = level.IsSubLevel,
            Score = level.CoolRating,
            PhotosTaken = level.Statistics.PhotoInLevelCount,
            LevelComments = level.Statistics.CommentCount,
            Reviews = level.Statistics.ReviewCount,
            Tags = dataContext.Database.GetTagsForLevel(level).Select(t => t.Tag),
            IsModded = level.IsModded,
        };
    }
    
    private static string GetIconHash(GameLevel level, DataContext dataContext)
    {
        string hash = dataContext.Database.GetAssetFromHash(level.IconHash)?.GetAsIcon(TokenGame.Website, dataContext) ?? level.IconHash;
        return level.GameVersion == TokenGame.LittleBigPlanetPSP
            ? "psp/" + hash
            : hash;
    }

    public static IEnumerable<ApiGameLevelResponse> FromOldList(IEnumerable<GameLevel> oldList, DataContext dataContext) => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}