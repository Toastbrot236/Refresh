using MongoDB.Bson;
using Refresh.Database.Models.Integrations;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Relations;

#nullable disable

/// <summary>
/// Used to cache who the code belongs to, according to the authority verified against, to avoid duplicate requests to said authority.
/// </summary>
[PrimaryKey(nameof(Code), nameof(FromAuthority))]
public partial class CachedOAuthCodeRelation
{
    public string Code { get; set; }
    public OAuthAuthority FromAuthority  { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [Required] public GameUser User { get; set; }
    [Required] public ObjectId UserId { get; set; }
}