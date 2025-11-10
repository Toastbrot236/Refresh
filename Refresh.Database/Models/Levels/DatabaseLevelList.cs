using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Levels;

/// <summary>
/// A DatabaseList of levels which may also include users independently.
/// </summary>
public class DatabaseLevelList : DatabaseList<GameLevel>
{
    public DatabaseLevelList(IEnumerable<GameLevel> items, int skip, int count, IEnumerable<GameUser> users) : base(items, skip, count)
    {
        this.Users = users;
    }

    public IEnumerable<GameUser> Users { get; private init; }
}