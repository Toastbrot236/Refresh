using Bunkum.Core;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Relations;
using Refresh.Database.Models.Users;
using Refresh.Database.Query;

namespace Refresh.Core.Types.Categories.Levels;

public class OverrideLevelsCategory : GameCategory
{
    internal OverrideLevelsCategory() : base("overridingLevels", [], false)
    {
        this.Name = "Play Now Levels";
        this.Description = "Levels and level hashes you've used Play Now on recently.";
        this.FontAwesomeIcon = "play";
        this.IconHash = "g820608";
        this.PrimaryResultType = ResultType.Level;
    }

    public override DatabaseResultList? Fetch(RequestContext context, int skip, int count,
        DataContext dataContext,
        LevelFilterSettings _, GameUser? user)
    {
        if (user == null) return null;
        DatabaseList<LevelOverrideRelation> overrides = dataContext.Database.GetLevelsQueuedByUser(user, skip, count);
    }
}