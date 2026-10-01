using MongoDB.Bson;

namespace Refresh.Database.Models.Statistics;

public class GameUserStatistics
{
    [Required, Key] public ObjectId UserId { get; set; }
    public DateTimeOffset? RecalculateAt { get; set; } = null;
    public int Version { get; set; } = GameDatabaseContext.UserStatisticsVersion;
    
    public int FavouriteCount { get; set; }
    public int CommentCount { get; set; }
    public int LevelCount { get; set; }
    public int PhotosByUserCount { get; set; }
    public int PhotosWithUserCount { get; set; }
    public int ReviewCount { get; set; }
    public int FavouriteLevelCount { get; set; }
    public int FavouriteUserCount { get; set; }
    public int FavouritePlaylistCount { get; set; }
    public int QueueCount { get; set; }
    public int PlaylistCount { get; set; }
    public int TotalPlayCount { get; set; }
    public int TotalCompletionCount { get; set; }
    public int UniquePlayCount { get; set; }
    public int UniqueCompletionCount { get; set; }
    public long TotalPlayTimeMinutes { get; set; }
}