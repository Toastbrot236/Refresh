using Bunkum.Core;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.Database.Query;

namespace Refresh.Core.Types.Categories.Levels;

public class HeartedLevelsByUserCategory : GameLevelCategory
{
    internal HeartedLevelsByUserCategory() : base("hearted", "favouriteSlots", true)
    {
        this.Name = "My Favorites";
        this.Description = "Your personal list filled with your favourite levels!";
        this.FontAwesomeIcon = "heart";
        this.IconHash = "g820611";
    }
    
    public override DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? user)
    {
        // Prefer specified user from query, but fallback to requesting user if it's missing
        user = this.GetUserFromQuery(context, dataContext.Database) ?? user;
        if (user == null) return null;
        
        return dataContext.Database.GetLevelsFavouritedByUser(user, count, skip, levelFilterSettings, dataContext.User);
    }
}