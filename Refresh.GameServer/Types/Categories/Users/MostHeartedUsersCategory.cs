using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Categories.Users;

public class MostHeartedUsersCategory : UserCategory
{
    internal MostHeartedUsersCategory() : base("mostHeartedUsers", [], false)
    {
        this.Name = "Most Hearted Users";
        this.Description = "The most beloved users in the community!";
        this.FontAwesomeIcon = "heart";
        this.IconHash = "g820611";
    }
    
    public override DatabaseList<GameUser>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
        => new
        (
            dataContext.Database.GetMostHeartedUsers(), 
            skip, 
            count
        );
}