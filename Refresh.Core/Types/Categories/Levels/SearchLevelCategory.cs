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

    public override DatabaseLevelList? Fetch(RequestContext context, int skip, int count,
        DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
    {
        string? query = context.QueryString["query"]
                        ?? context.QueryString["textFilter"]; // LBP3 sends this instead of query
        if (query == null) return null;

        // Get custom commands from query, remove them from query and then use them.
        List<string> queryParts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        SearchQueryParameters queryParameters = new();

        for (int i = 0; i < queryParts.Count; i++)
        {
            string part = queryParts[i];

            // If this is a command, find out which command this is. 
            // If it's a valid command, remove it after applying it.
            if (part.Length >= 2 && SyntaxChars.ParamPrefixes.Contains(part[0]))
            {
                string command = part[1..];
                bool isCommand = false;

                if (command.Equals("nt", StringComparison.InvariantCultureIgnoreCase)
                    || command.Equals("notitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    queryParameters.ExcludeTitle = true;
                    isCommand = true;
                }
                else if (command.Equals("nd", StringComparison.InvariantCultureIgnoreCase)
                    || command.Equals("nodescription", StringComparison.InvariantCultureIgnoreCase))
                {
                    queryParameters.ExcludeDescription = true;
                    isCommand = true;
                }
                // Incase a LBP1 or 2 user wants to only see levels or only users.
                // Only allow this for the game for now.
                else if (!context.IsApi())
                {
                    if (command.Equals("nu", StringComparison.InvariantCultureIgnoreCase)
                        || command.Equals("nousers", StringComparison.InvariantCultureIgnoreCase))
                    {
                        levelFilterSettings.DisplayUsers = false;
                        isCommand = true;
                    }
                    else if (command.Equals("nl", StringComparison.InvariantCultureIgnoreCase)
                        || command.Equals("nolevels", StringComparison.InvariantCultureIgnoreCase))
                    {
                        levelFilterSettings.DisplayLevels = false;
                        isCommand = true;
                    }
                }

                // Remove substring from query if it was a command
                if (isCommand)
                {
                    queryParts.RemoveAt(i);
                    i--; // Prevent skipping over a part after removal
                    continue;
                }
            }
        }

        // Add the query to the struct, but by rebuilding it off of the remaining substrings to exclude commands.
        // Also has the useful side-effect of removing double and trailing whitespaces.
        queryParameters.Query = string.Join(' ', queryParts);

        // Actually fetch levels and users if requested
        DatabaseList<GameLevel>? levels = levelFilterSettings.DisplayLevels 
            ? dataContext.Database.SearchForLevels(skip, count, dataContext.User, levelFilterSettings, queryParameters)
            : null;

        DatabaseList<GameUser>? users = levelFilterSettings.DisplayUsers 
            ? dataContext.Database.SearchForUsers(skip, count, queryParameters)
            : null;

        return new(levels?.Items ?? [], skip, count, users?.Items ?? []);
    }
}