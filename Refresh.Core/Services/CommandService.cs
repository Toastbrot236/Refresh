using System.Diagnostics.CodeAnalysis;
using Bunkum.Core.Services;
using JetBrains.Annotations;
using MongoDB.Bson;
using NotEnoughLogs;
using Refresh.Common.Verification;
using Refresh.Core.Types.Commands;
using Refresh.Database;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;

namespace Refresh.Core.Services;

public class CommandService : EndpointService
{
    private readonly PlayNowService _levelListService;
    
    public CommandService(Logger logger, PlayNowService levelListService) : base(logger)
    {
        this._levelListService = levelListService;
    }

    private readonly HashSet<ObjectId> _usersPublishing = [];

    /// <summary>
    /// Start tracking the user, eg. they started publishing
    /// </summary>
    /// <param name="id">The user ID</param>
    public void StartPublishing(ObjectId id)
    {
        //Unconditionally add the user to the set
        this._usersPublishing.Add(id);
    }

    /// <summary>
    /// Stop tracking the user, eg. they stopped publishing
    /// </summary>
    /// <param name="id">The user ID</param>
    public void StopPublishing(ObjectId id)
    {
        //Unconditionally remove the user from the set
        this._usersPublishing.Remove(id);
    }

    public bool IsPublishing(ObjectId id) => this._usersPublishing.Contains(id);

    /// <summary>
    /// Parse a command string into a command object
    /// </summary>
    /// <param name="input">Command string</param>
    /// <returns>Parsed command</returns>
    /// <exception cref="FormatException">When the command is in an invalid format</exception>
    [Pure]
    public CommandInvocation ParseCommand(ReadOnlySpan<char> input)
    {
        // Ensure the command string starts with a slash
        if (input[0] != '/')
        {
            throw new FormatException("Commands must start with `/`");
        }

        int index = input.IndexOf(' ');

        // If index is 1, the command name is blank
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (index == 1)
        {
            throw new FormatException("Blank command name");
        }

        //If theres no space after, or if the space is the last character, then there are no arguments
        if (index == -1 || index == input.Length - 1)
        {
            return new CommandInvocation(index == input.Length - 1 ? input[1..index] : input[1..], null);
        }
        
        return new CommandInvocation(input[1..index], input[(index + 1)..]);
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void HandleCommand(CommandInvocation command, GameDatabaseContext database, GameUser user, Token token)
    {
        switch (command.Name)
        {
            case "forcematch":
            {
                if (command.Arguments.IsEmpty)
                {
                    throw new Exception("User not provided for force match command");
                }
                
                GameUser? target = database.GetUserByUsername(command.Arguments.ToString());

                if (target != null)
                {
                    database.SetForceMatch(user, target);
                }
                
                break;
            }
            case "clearforcematch":
            {
                database.ClearForceMatch(user);
                break;
            }
            case "unescapexmlon":
            {
                database.SetUnescapeXmlSequences(user, true);
                break;
            }
            case "unescapexmloff":
            {
                database.SetUnescapeXmlSequences(user, false);
                break;
            }
            case "showmods":
            {
                database.SetShowModdedContent(user, true);
                break;
            }
            case "hidemods":
            {
                database.SetShowModdedContent(user, false);
                break;
            }
            case "showreupload":
            case "showreuploads":
            {
                database.SetShowReuploadedContent(user, true);
                break;
            }
            case "hidereupload":
            case "hidereuploads":
            {
                database.SetShowReuploadedContent(user, false);
                break;
            }
            case "play":
            {
                if (CommonPatterns.Sha1Regex().IsMatch(command.Arguments))
                {
                    this._levelListService.PlayNowHash(user, command.Arguments.ToString());
                }
                else
                {
                    
                    GameLevel? level = database.GetLevelById(int.Parse(command.Arguments));
                    if (level != null)
                    {
                        this._levelListService.PlayNowLevel(user, level);
                    }
                }
                
                break;
            }
            case "beta":
            {
                database.ForceUserTokenGame(token, TokenGame.BetaBuild);
                break;
            }
            case "revoketoken":
            {
                database.RevokeToken(token);
                break;
            }
            #if DEBUG
            case "tokengame":
            {
                database.ForceUserTokenGame(token, (TokenGame)int.Parse(command.Arguments));
                break;
            }
            case "tokenplatform":
            {
                database.ForceUserTokenPlatform(token, (TokenPlatform)int.Parse(command.Arguments));
                break;
            }
            case "notif":
            {
                database.AddNotification("Debug", "This is a debug notification triggered by a command.", user);
                break;
            }
            #endif
        }
    }
}