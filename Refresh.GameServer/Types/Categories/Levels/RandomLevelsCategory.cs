using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Categories.Levels;

public class RandomLevelsCategory : GameLevelCategory
{
    internal RandomLevelsCategory() : base("randomLevels", ["lbp2luckydip", "luckydip"], false)
    {
        this.Name = "Lucky Dip";
        this.Description = "A random assortment of levels!";
        this.FontAwesomeIcon = "shuffle";
        this.IconHash = "g820605";
    }
    
    public override DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _) 
        => dataContext.Database.GetRandomLevels(count, skip, dataContext.User, levelFilterSettings);
}