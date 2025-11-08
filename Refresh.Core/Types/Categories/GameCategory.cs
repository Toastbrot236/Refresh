using Bunkum.Core;
using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Playlists;
using Refresh.Database.Models.Users;

namespace Refresh.Core.Types.Categories;

[JsonObject(MemberSerialization.OptIn)]
public abstract class GameCategory
{
    [JsonProperty] public string Name { get; set; } = "";
    [JsonProperty] public string Description { get; set; } = "";
    [JsonProperty] public string IconHash { get; set; } = "0";
    [JsonProperty] public string FontAwesomeIcon { get; set; } = "faCertificate";
    [JsonProperty] public bool Hidden { get; set; } = false;

    [JsonProperty] public readonly bool RequiresUser;
    [JsonProperty] public readonly string ApiRoute;
    public readonly string[] GameRoutes;
    
    internal GameCategory(string apiRoute, string gameRoute, bool requiresUser) : this(apiRoute, [gameRoute], requiresUser) {}
    internal GameCategory(string apiRoute, string[] gameRoutes, bool requiresUser)
    {
        this.ApiRoute = apiRoute;
        this.GameRoutes = gameRoutes;
        
        this.RequiresUser = requiresUser;
    }

    protected GameUser? GetUserFromQuery(RequestContext context, GameDatabaseContext database)
    {
        string? username = context.QueryString["u"] ?? context.QueryString["username"];
        GameUser? user = username != null ? database.GetUserByUsername(username, false) : null;
        if (user != null) return user;

        // If this is an API request and user isn't specified by name, lookup the UUID param
        if (!context.IsApi()) return null;

        string? userId = context.QueryString["user"];
        return userId != null ? database.GetUserByUuid(userId) : null;
    }

    protected GameLevel? GetLevelFromQuery(RequestContext context, GameDatabaseContext database)
    {
        string? levelId = context.QueryString["l"] ?? context.QueryString["level"];
        if (levelId == null) return null;
        
        bool parsed = int.TryParse(levelId, out int id);
        return parsed ? database.GetLevelById(id) : null;
    }

    protected GamePlaylist? GetPlaylistFromQuery(RequestContext context, GameDatabaseContext database)
    {
        string? playlistId = context.QueryString["p"] ?? context.QueryString["playlist"];
        if (playlistId != null) return null;
        
        bool parsed = int.TryParse(playlistId, out int id);
        return parsed ? database.GetPlaylistById(id) : null;
    }
}