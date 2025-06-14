using MongoDB.Bson;

namespace Refresh.Database.Models.Users;

#nullable disable
#if POSTGRES
using PrimaryKeyAttribute = Microsoft.EntityFrameworkCore.PrimaryKeyAttribute;
[PrimaryKey(nameof(UserId), nameof(Code))]
#endif
public partial class EmailVerificationCode : IRealmObject
{
    [ForeignKey(nameof(UserId))]
    public GameUser User { get; set; }
    public string Code { get; set; }
    
    [Ignored] public ObjectId UserId { get; set; }
    
    public DateTimeOffset ExpiryDate { get; set; }
}