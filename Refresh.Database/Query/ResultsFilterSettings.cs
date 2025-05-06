using Bunkum.Core;
using Refresh.Database.Models.Authentication;
using Refresh.GameServer.Extensions;

namespace Refresh.Database.Query.Filtering;

public class ResultsFilterSettings
{
    // result type
    public bool DisplayLevels { get; set; }
    public bool DisplayPlaylists { get; set; }
    public bool DisplayUsers { get; set; }

    // level game versions
    public bool DisplayLbp1 { get; set; }
    public bool DisplayLbp2 { get; set; }
    public bool DisplayLbp3 { get; set; }
    public bool DisplayVita { get; set; }
    public bool DisplayPsp { get; set; }
    public bool DisplayBeta { get; set; }

    // level properties
    public PropertyFilterType DisplayMoveLevels { get; set; }
    public bool DisplayAdventures { get; set; }
    public bool? DisplayModdedLevels { get; set; }
    public byte? Players { get; set; }
    public byte? MinPlayers { get; set; }
    public byte? MaxPlayers { get; set; }
    public string[] Labels { get; set; } = [];

    // user relations
    public bool? IncludeMyLevels { get; set; }
    public bool? IncludePlayedLevels { get; set; }

    // other
    public int? Seed { get; set; }

    public static ResultsFilterSettings FromRequest(RequestContext context, TokenGame game)
    {
        if (context.IsApi())
            return FromApiRequest(context, game);
        else
            return FromGameRequest(context, game);
    }

    /// <summary>
    /// Gets the filter settings from a request sent by a game which is not LBP3
    /// </summary>
    public static ResultsFilterSettings FromGameRequest(RequestContext context, TokenGame game)
    {
        ResultsFilterSettings settings = new()
        {
            DisplayLbp1 = false,
            DisplayLbp2 = false,
            DisplayLbp3 = false,
            DisplayVita = false,
            DisplayPsp = false,
            DisplayBeta = false,
            
            IncludeMyLevels = true,
            IncludePlayedLevels = true,

            DisplayLevels = true,
            DisplayPlaylists = false,
            DisplayUsers = false,

            DisplayMoveLevels = PropertyFilterType.Include,
            DisplayAdventures = false,
        };

        bool gamesSpecified = false;
        string[]? gameFilters = context.QueryString.GetValues("gameFilter[]");
        string? gameFilterType = context.QueryString.Get("gameFilterType");

        if (game == TokenGame.BetaBuild)
        {
            settings.DisplayBeta = true;
        }
        else if (gameFilters != null)
        {
            gamesSpecified = true;
            foreach (string gameFilter in gameFilters)
            {
                switch (gameFilter)
                {
                    case "lbp1":
                        settings.DisplayLbp1 = true;
                        break;
                    case "lbp2":
                        settings.DisplayLbp2 = true;
                        break;
                    case "lbp3":
                        settings.DisplayLbp3 = true;
                        break;
                }
            }
        }
        else if (gameFilterType != null)
        {
            switch (gameFilterType)
            {
                case "lbp1":
                    settings.DisplayLbp1 = true;
                    settings.DisplayLbp2 = false;
                    break;
                case "lbp2":
                    settings.DisplayLbp1 = false;
                    settings.DisplayLbp2 = true;
                    break;
                case "both":
                    settings.DisplayLbp1 = true;
                    settings.DisplayLbp2 = true;
                    break;
                default: 
                    throw new ArgumentOutOfRangeException(nameof(gameFilterType), gameFilterType, "Unsupported value");
            };
        }
        else
        {
            switch (game)
            {
                case TokenGame.LittleBigPlanet1:
                    settings.DisplayLbp1 = true;
                    break;
                case TokenGame.LittleBigPlanet2:
                    settings.DisplayLbp1 = true;
                    settings.DisplayLbp2 = true;
                    break;
                case TokenGame.LittleBigPlanet3:
                    settings.DisplayLbp1 = true;
                    settings.DisplayLbp2 = true;
                    settings.DisplayLbp3 = true;
                    break;
                case TokenGame.LittleBigPlanetVita:
                    settings.DisplayVita = true;
                    break;
                case TokenGame.LittleBigPlanetPSP:
                    settings.DisplayPsp = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, "Unsupported value");
            }
        }

        string[]? resultTypes = context.QueryString.GetValues("resultType[]");
        if (resultTypes != null)
        {
            foreach (string resultType in resultTypes)
            {
                switch (resultType)
                {
                    case "slot":
                        settings.DisplayLevels = true;
                        break;
                    case "playlist":
                        settings.DisplayPlaylists = true;
                        break;
                    case "user":
                        settings.DisplayUsers = true;
                        break;
                }
            }
        }

        string? move = context.QueryString.Get("move");
        if (move != null)
        {
            switch (move)
            {
                // LBP2 options
                case "true":
                    settings.DisplayMoveLevels = PropertyFilterType.Include;
                    break;
                case "false":
                    settings.DisplayMoveLevels = PropertyFilterType.Exclude;
                    break;
                case "only":
                    if (settings.DisplayLbp1)
                        settings.DisplayMoveLevels = PropertyFilterType.Include;
                    else
                        settings.DisplayMoveLevels = PropertyFilterType.Only;
                    break;
                // LBP3 options
                case "dontCare":
                    // atleast one game selected, but move unselected -> exclude move levels
                    if (gamesSpecified)
                        settings.DisplayMoveLevels = PropertyFilterType.Exclude;
                    // no games selected, move also unselected (default settings) -> show all levels
                    else
                        settings.DisplayMoveLevels = PropertyFilterType.Include;
                    break;
                case "allMust":
                    // atleast one game selected, move also selected -> show all levels of these games
                    if (gamesSpecified)
                        settings.DisplayMoveLevels = PropertyFilterType.Include;
                    // no games selected, but move selected -> only show move levels
                    else
                        settings.DisplayMoveLevels = PropertyFilterType.Only;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, "Unsupported value");
            }
        }

        string? adventure = context.QueryString.Get("adventure");
        if (adventure != null)
        {
            settings.DisplayAdventures = adventure switch
            {
                "noneCan" => false,
                "dontCare" => true,
                _ => throw new ArgumentOutOfRangeException(nameof(adventure), adventure, "Unsupported value"),
            };
        }
        
        string? playersStr = context.QueryString.Get("players");
        if (playersStr != null && byte.TryParse(playersStr, out byte players))
        {
            settings.Players = players;
        }
            
        // level labels are currently not supported anyway, so leave as empty array
        //string? labelFilter0 = context.QueryString.Get("labelFilter0");
        //string? labelFilter1 = context.QueryString.Get("labelFilter1");
        //string? labelFilter2 = context.QueryString.Get("labelFilter2");

        string? seedStr = context.QueryString.Get("seed");
        if (seedStr != null && int.TryParse(seedStr, out int seed))
        {
            settings.Seed = seed;
        }

        return settings;
    }

    public static ResultsFilterSettings FromApiRequest(RequestContext context, TokenGame game)
    {
        return new();
    }
}