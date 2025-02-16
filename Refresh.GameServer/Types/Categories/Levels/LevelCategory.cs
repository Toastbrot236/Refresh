using System.Diagnostics.Contracts;
using System.Xml.Serialization;
using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.Lists;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Categories.Levels;

[JsonObject(MemberSerialization.OptIn)]
public abstract class LevelCategory : Category
{
    internal LevelCategory(string apiRoute, string gameRoute, bool requiresUser) : base(apiRoute, new []{gameRoute}, requiresUser) {}
    
    internal LevelCategory(string apiRoute, string[] gameRoutes, bool requiresUser) : base(apiRoute, gameRoutes, requiresUser) {}

    [Pure]
    public abstract DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? user);
}