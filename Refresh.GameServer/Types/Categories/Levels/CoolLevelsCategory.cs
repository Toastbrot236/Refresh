using Bunkum.Core;
using Refresh.Database.Query;
using Refresh.Database;
using Refresh.GameServer.Types.Data;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels;

namespace Refresh.GameServer.Types.Categories.Levels;

public class CoolLevelsCategory : GameLevelCategory
{
    public CoolLevelsCategory() : base("coolLevels", ["lbpcool", "lbp2cool", "cool"], false)
    {
        this.Name = "Cool Levels";
        this.Description = "Levels trending with players like you!";
        this.FontAwesomeIcon = "fire";
        this.IconHash = "g820625";
    }

    public override DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
    {
        return dataContext.Database.GetCoolLevels(count, skip, dataContext.User, levelFilterSettings);
    }
}