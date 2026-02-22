using Bunkum.Core;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.Database.Query;

namespace Refresh.Core.Types.Categories.Levels;

public class SearchLevelCategory : GameCategory
{
    internal SearchLevelCategory() : base("search", "search", false)
    {
        this.Name = "Search";
        this.Description = "Search for new levels.";
        this.FontAwesomeIcon = "magnifying-glass";
        // no icon for now, too lazy to find
        this.Hidden = true; // The search category is not meant to be shown, as it requires a special implementation on all frontends
        this.PrimaryResultType = ResultType.Level;
    }

    public override DatabaseResultList? Fetch(RequestContext context, int skip, int count,
        DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
    {
        string? query = context.QueryString["query"]
                        ?? context.QueryString["textFilter"]; // LBP3 sends this instead of query
        if (query == null) return null;

        // TODO: use separate class(es) to model parameters (and also chat commands) so we could send all available commands/parameters
        // together with summaries of what they do over API endpoints for a documentation page
        //
        // Look for parameters
        // Split by whitespaces, remove all substrings which are parameters, concatenate remaining substrings into the final query.
        // Also gets rid of accidental leading/trailing/double whitespaces.
        string[] queryParts = query.Split(' ');
        List<string> queryPartsFinal = [];
        bool showModded = dataContext.User?.ShowModdedContent ?? true;
        bool showReuploaded = dataContext.User?.ShowReuploadedContent ?? true;
        bool showLevels = true;
        bool showUsers = true;
        bool compareNames = true;
        bool compareDescriptions = true;
        // TODO showPlaylists
        foreach(String queryPart in queryParts)
        {
            // Commands must start with a prefix
            if (queryPart.Length < 2 || (!queryPart.StartsWith('.') && !queryPart.StartsWith('-')))
            {
                queryPartsFinal.Add(queryPart);
                continue;
            }
            switch (queryPart[1..].ToLower())
            {
                case "nm":
                case "nomodded":
                    showModded = false;
                    break;
                case "nr":
                case "noreupload":
                    showReuploaded = false;
                    break;
                case "nl":
                case "nolevel":
                    showLevels = false;
                    break;
                case "nu":
                case "nouser":
                    showUsers = false;
                    break;
                case "nn":
                case "noname":
                    compareNames = false;
                    break;
                case "nd":
                case "nodescription":
                    compareDescriptions = false;
                    break;
                default:
                    queryPartsFinal.Add(queryPart);
                    break;
            }
        }

        string finalQuery = $"%{string.Join(' ', queryPartsFinal)}%";
        levelFilterSettings.ShowModdedLevels = showModded;
        levelFilterSettings.ShowReuploadedLevels = showReuploaded;
        levelFilterSettings.SearchSettings = new()
        {
            CompareNames = compareNames,
            CompareDescriptions = compareDescriptions,
        };

        DatabaseList<GameLevel>? levels = showLevels ? dataContext.Database.SearchForLevels(count, skip, dataContext.User, levelFilterSettings, finalQuery) : null;
        DatabaseList<GameUser>? users = showUsers ? dataContext.Database.SearchForUsers(count, skip, levelFilterSettings, finalQuery) : null;

        return new()
        {
            Levels = levels,
            Users = users,
            Playlists = null, // TODO
        };
    }
}