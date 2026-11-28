using MongoDB.Bson;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Levels.Scores;

[PrimaryKey(nameof(ScoreId), nameof(PlayerNumber))]
public partial class GameScorePlayer
{
    [Required]
    public ObjectId ScoreId { get; set; }
    [Required, ForeignKey(nameof(ScoreId))]
    public GameScore Score { get; set; } = null!;

    public int PlayerNumber { get; set; }

    // TODO allow storing usernames of users we don't know aswell, by adding a username column
    // and making UserId nullable. This'd be useful to preserve the names of guest users from another server.
    // We don't do this right now because showing these userless names isn't possible in APIv3, so they're not moderateable.
    // Should wait for APIv4 first.

    [Required, ForeignKey(nameof(UserId))]
    public GameUser User { get; set; } = null!;
    [Required]
    public ObjectId UserId { get; set; }
}