using System.Text;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Endpoints.Debugging;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Configuration;
using Refresh.Core.Services;
using Refresh.Core.Types.Data;
using Refresh.Core.Types.Matching;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;

namespace Refresh.Interfaces.Game.Endpoints;

public class MatchingEndpoints : EndpointGroup
{
    public static string FixupLocationData(string body)
    {
        StringBuilder jsonBodyBuilder = new();

        const string locationStart = "\"Location\":[";
        const string corruptedStr = "\"0.0.0.0\"";
        
        int locationIndex = body.IndexOf(locationStart, StringComparison.InvariantCulture) + locationStart.Length;
        // Append the start of the "location" mess
        jsonBodyBuilder.Append(body.AsSpan()[..locationIndex]);
        ReadOnlySpan<char> pastLocationStart = body.AsSpan()[locationIndex..];

        for (int i = 0; i < pastLocationStart.Length;)
        {
            ReadOnlySpan<char> slice = pastLocationStart[i..];

            int corruptedStart = slice.IndexOf(corruptedStr);
            // If theres no more corrupted strings, then we are done fixing it up
            if (corruptedStart == -1)
            {
                jsonBodyBuilder.Append(slice);
                break;
            }

            int corruptedEnd = corruptedStart + corruptedStr.Length;
            char charAfterCorruption = slice[corruptedEnd];

            switch (charAfterCorruption)
            {
                // If this is the start to a string, then we know that a `,` was corrupted
                case '\"':
                    jsonBodyBuilder.Append(slice[..corruptedEnd]);
                    jsonBodyBuilder.Append(',');
                    i += corruptedEnd;
                    continue;
                // If this is a comma or end brace, then we know that a ']' was corrupted
                case ',':
                case '}':
                    jsonBodyBuilder.Append(slice[..corruptedEnd]);
                    jsonBodyBuilder.Append(']');
                    i += corruptedEnd;
                    continue;
            }
            
            i++;
        }

        return jsonBodyBuilder.ToString();
    }
    
    // [FindBestRoom,["Players":["VitaGamer128"],"Reservations":["0"],"NAT":[2],"Slots":[[5,0]],"Location":[0x17257bc9,0x17257bf2],"Language":1,"BuildVersion":289,"Search":"","RoomState":3]]
    [GameEndpoint("match", HttpMethods.Post, ContentType.Json)]
    [DebugRequestBody, DebugResponseBody]
    [RequireEmailVerified]
    public Response Match(
        RequestContext context, 
        string body, 
        DataContext dataContext,
        GameServerConfig gameServerConfig)
    {
        if (dataContext.User!.IsWriteBlocked(gameServerConfig))
            return Unauthorized;
        
        (string method, string rawJsonBody) = MatchService.ExtractMethodAndBodyFromJson(body);

        string jsonBody = FixupLocationData(rawJsonBody);

        context.Logger.LogInfo(BunkumCategory.Matching, $"Received {method} match request, data: {jsonBody}");

        JsonSerializer serializer = new();
        using StringReader reader = new(jsonBody);
        using JsonTextReader jsonReader = new(reader);

        SerializedRoomData? roomData = serializer.Deserialize<SerializedRoomData>(jsonReader);
        
        // ReSharper disable once InvertIf (happy path goes last)
        if (roomData == null)
        {
            context.Logger.LogWarning(BunkumCategory.Matching, "Match data was bad and unserializable, rejecting."); // Already logged data
            return BadRequest;
        }
        
        return dataContext.Match.ExecuteMethod(method, roomData, dataContext, gameServerConfig);
    }
    
    // Sent periodically by LBP1 to notify the server it has entered a level.
    // While it doesn't send us any detailed room information, we can at least create a pseudo-room on the server with just that player.
    // Due to LBP1 requesting this endpoint way less often than /notification, it's much more likely for the room to be auto cleared due to inactivity,
    // since the "bump" is much less often. This will at the very least make API tools be able to see LBP1 player activity and player counts.
    // LBP1 doesn't send any requests to /play if a user enters an online user level, so use this endpoint to increment plays for those.
    [GameEndpoint("enterLevel/{slotType}/{id}", HttpMethods.Post)]
    public Response EnterLevel(RequestContext context, Token token, MatchService matchService, string slotType, int id, DataContext dataContext)
    {
        GameRoom room = matchService.GetOrCreateRoomByPlayer(token.User, token.TokenPlatform, token.TokenGame, NatType.Strict, false);
        
        // User slot ID of 0 means pod/moon level
        if (id == 0 && slotType == "user")
        {
            // We cant determine whether the user is in a pod or moon level, so we just assume it's the pod
            room.LevelType = RoomSlotType.Pod;
            room.LevelId = id;
        }
        else
        {
            GameLevel? level = dataContext.Database.GetLevelByIdAndType(slotType, id);
            RoomSlotType roomSlotType = slotType == "user" ? RoomSlotType.Online : RoomSlotType.Story;

            // User has actually entered a new online user level
            if (level != null && roomSlotType == RoomSlotType.Online && room.LevelId != id && room.LevelType != roomSlotType)
            {
                dataContext.Database.PlayLevel(level, token.User, 1);
            }

            room.LevelType = roomSlotType;
            room.LevelId = id;
        }
        
        room.LastContact = DateTimeOffset.Now;
        matchService.RoomAccessor.UpdateRoom(room);
        
        return OK;
    }
}