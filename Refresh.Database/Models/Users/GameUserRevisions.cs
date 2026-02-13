using MongoDB.Bson;

namespace Refresh.Database.Models.Users;

/// <summary>
/// A snapshot of a user's details at a point in time. Useful for moderation, or the ability to attribute certain things to users even if those
/// are only using an outdated username as reference. Currently only stores the usernames of a user.
/// </summary>
[PrimaryKey(nameof(RevisionId), nameof(UserId))]
public class GameUserRevision
{
    /// <summary>
    /// The sequential revision ID for this revision.
    /// </summary>
    [Required] public int RevisionId { get; set; }
    
    /// <summary>
    /// The ID of the user whose snapshot this is. Not marked as foreign key so that revisions could persist even if their users get fully deleted.
    /// </summary>
    [Required] public ObjectId UserId { get; set; }
    
    /// <summary>
    /// The point in time in which this revision was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// The username the user had before creating this revision.
    /// </summary>
    public string Username { get; set; } = "";
}