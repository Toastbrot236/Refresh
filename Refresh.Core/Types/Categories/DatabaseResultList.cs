using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Playlists;
using Refresh.Database.Models.Users;

namespace Refresh.Core.Types.Categories;

public class DatabaseResultList
{
    public DatabaseList<GameLevel>? Levels { get; set; } = null;
    public DatabaseList<GameUser>? Users { get; set; } = null;
    public DatabaseList<GamePlaylist>? Playlists { get; set; } = null;

    /// <summary>
    /// The TotalItems value of the DatabaseList with the highest TotalItems, so clients could load all pages, even
    /// if e.g. there are 20 users, 40 levels, and page size is 10.
    /// </summary>
    public int TotalItemsMax => Math.Max(Math.Max(this.Levels?.TotalItems ?? 0, this.Users?.TotalItems ?? 0), this.Playlists?.TotalItems ?? 0);
    
    /// <summary>
    /// The NextPageIndex value of the DatabaseList with the highest NextPageValue.
    /// If a DatabaseList has the last items of its list, its NextPageIndex will be 0, so we can simply take the highest one.
    /// </summary>
    public int NextPageIndexMax => Math.Max(Math.Max(this.Levels?.NextPageIndex ?? 0, this.Users?.NextPageIndex ?? 0), this.Playlists?.NextPageIndex ?? 0);
    
    public DatabaseResultList(DatabaseList<GameLevel> levels)
    {
        Levels = levels;
    }

    public DatabaseResultList(DatabaseList<GameUser> users)
    {
        Users = users;
    }

    public DatabaseResultList(DatabaseList<GamePlaylist> playlists)
    {
        Playlists = playlists;
    }

    public DatabaseResultList(DatabaseList<GameLevel>? levels, DatabaseList<GameUser>? users, DatabaseList<GamePlaylist>? playlists)
    {
        this.Levels = levels;
        this.Users = users;
        this.Playlists = playlists;
    }
}