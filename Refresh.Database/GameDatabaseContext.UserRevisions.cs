using Refresh.Database.Models.Users;

namespace Refresh.Database;

public partial class GameDatabaseContext // UserRevisions
{
    public GameUserRevision CreateRevisionForUser(GameUser user, bool saveChanges = false)
    {
        // FIXME: see comment in CreateRevisionForLevel()
        int sequentialId = this.GameUserRevisions
            .Where(r => r.UserId == user.UserId)
            .DefaultIfEmpty()
            .Max(r => r != null ? r.RevisionId : 0);
        
        GameUserRevision revision = new()
        {
            UserId = user.UserId,
            CreatedAt = this._time.Now,
            
            RevisionId = sequentialId + 1,
            Username = user.Username
        };

        this.GameUserRevisions.Add(revision);

        if(saveChanges)
            this.SaveChanges();

        return revision;
    }
}