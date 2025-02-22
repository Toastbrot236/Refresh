using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Playlists;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Categories.Playlists;

public class NewestPlaylistsCategory : PlaylistCategory
{
    internal NewestPlaylistsCategory() : base("newestPlaylists", [], false)
    {
        this.Name = "Newest Playlists";
        this.Description = "Our newest user-made playlists.";
        this.IconHash = "g820623";
        this.FontAwesomeIcon = "calendar";
    }
    
    public override DatabaseList<GamePlaylist>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _) 
        => new DatabaseList<GamePlaylist>(
            dataContext.Database.GetNewestPlaylists(),
            skip,
            count
        );
}