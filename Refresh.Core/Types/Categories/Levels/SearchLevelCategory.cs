using Bunkum.Core;
using Refresh.Common.Constants;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.Database.Query;

namespace Refresh.Core.Types.Categories.Levels;

public class SearchLevelCategory : GameLevelCategory
{
    internal SearchLevelCategory() : base("search", "search", false)
    {
        this.Name = "Search";
        this.Description = "Search for new levels.";
        this.FontAwesomeIcon = "magnifying-glass";
        // no icon for now, too lazy to find
        this.Hidden = true; // The search category is not meant to be shown, as it requires a special implementation on all frontends
    }

    public override DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count,
        DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
    {
        string? query = context.QueryString["query"]
                        ?? context.QueryString["textFilter"]; // LBP3 sends this instead of query
        if (query == null) return null;

        // Get custom params from search query, remove them from query and then use them.
        List<string> querySubstrings = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        SearchQueryParameters searchParams = new();

        for (int i = 0; i < querySubstrings.Count; i++)
        {
            string substring = querySubstrings[i];

            // If this is a param, find out which one it is.
            if (substring.Length >= 2 && SyntaxChars.SearchParamPrefixes.Contains(substring[0]))
            {
                string param = substring[1..];
                bool isParam = true;

                switch (param)
                {
                    case "nt":
                    case "notitle":
                        searchParams.ExcludeTitle = true;
                        break;
                    case "nd":
                    case "nodescription":
                        searchParams.ExcludeDescription = true;
                        break;
                    // TODO: Enable and use the two below once showing users in slot responses is implemented,
                    // but probably also keep them for LBP3/API even though there are/will be dedicated query params for that
#if false
                    case "nl":
                    case "nolevels":
                        levelFilterSettings.DisplayLevels = false;
                        break;
                    case "nu":
                    case "nousers":
                        levelFilterSettings.DisplayUsers = false;
                        break;
#endif
                    default:
                        isParam = false;
                        break;
                }

                // Remove substring from query if it was a valid search param
                if (isParam)
                {
                    querySubstrings.RemoveAt(i);
                    i--; // Prevent skipping over a substring after removal
                    continue;
                }
            }
        }

        // Recombine param-less substrings back to a string. 
        // This approach also has the useful side-effect of removing double and trailing whitespaces.
        searchParams.Query = string.Join(' ', querySubstrings);

        // Actually fetch levels
        DatabaseList<GameLevel> levels = dataContext.Database.SearchForLevels(count, skip, dataContext.User, levelFilterSettings, searchParams);

        return levels;
    }
}