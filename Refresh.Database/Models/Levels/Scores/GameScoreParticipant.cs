using MongoDB.Bson;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Levels.Scores;

[PrimaryKey(nameof(ScoreId), nameof(PlayerNumber))]
public partial class GameScoreParticipant
{
    [Required]
    public ObjectId ScoreId { get; set; }
    [Required, ForeignKey(nameof(ScoreId))]
    public GameScore Score { get; set; } = null!;

    public int PlayerNumber { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public GameUser? User { get; set; }
    public ObjectId? UserId { get; set; }
    public string Username { get; set; } = "";
}