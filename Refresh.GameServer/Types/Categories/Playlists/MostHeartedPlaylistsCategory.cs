using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Playlists;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Categories.Playlists;

public class MostHeartedPlaylistsCategory : PlaylistCategory
{
    internal MostHeartedPlaylistsCategory() : base("mostHeartedPlaylists", [], false)
    {
        this.Name = "Most Hearted Playlists";
        this.Description = "The most hearted level collections!";
        this.FontAwesomeIcon = "heart";
        this.IconHash = "g820611";
    }
    
    public override DatabaseList<GamePlaylist>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _) 
        => new
        (
            dataContext.Database.GetMostHeartedPlaylists(), 
            skip, 
            count
        );
}