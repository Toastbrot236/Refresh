using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels;

namespace Refresh.Database.Models.Comments;

#nullable disable

public partial class GameLevelComment : IGameComment, ISequentialId
{
    [Key] public int SequentialId { get; set; }

    /// <inheritdoc/>
    [Required]
    public GameUser Author { get; set; } = null!;

    /// <summary>
    /// The destination level this comment was posted to.
    /// </summary>
    [Required]
    public GameLevel Level { get; set; } = null!;
    
    /// <inheritdoc/>
    public string Content { get; set; } = string.Empty;
    
    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; set; }
}