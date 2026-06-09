using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Endpoints.Debugging;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.Common;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Services;
using Refresh.Core.Types.Telemetry;
using Refresh.Core.Types.Telemetry.Json;
using Refresh.Core.Types.Telemetry.Json.Events;
using Refresh.Database.Models.Users;
using Refresh.Core.Importing;

namespace Refresh.Interfaces.Game.Endpoints;

public class TelemetryEndpoints : EndpointGroup
{
    [GameEndpoint("t", HttpMethods.Post)]
    [MinimumRole(GameUserRole.Restricted)]
    [DebugRequestBody]
    public Response UploadBinaryTelemetry(RequestContext context, byte[] body, GameUser user, MatchService match)
    {
        if (body.Length > 8128) // 4032 in earlier versions, I guess
        {
            context.Logger.LogWarning(BunkumCategory.Game, $"User {user.Username} attempted to upload telemetry buffer above game's maximum size! This likely did not come from an official client.");
            return RequestEntityTooLarge;
        }
        
        // Probably wouldn't be handling all the parsing here normally,
        // but I'm only using this as a scratchpad really.
        MemoryBitStream reader = new(body);
        
        // Common revisions
            // LBP1 01.21 is 0x2 (Start is 0x0 instead of 0x1 in this version?)
            // LBP1 Deploy is 0x3 (Deploy is both before/after LBP1, branches)
            // LBP1 01.30-Final is 0xd
            // LBP2 Pre-Alpha is 0xe
            // LBP2 Move Beta is 0x19
            // LBP2 Final is 0x1f
            // LBP2 Vita Final is 0x1e
            // LBP2 Hub is 0x1e
            // LBP3 Alpha is 0x1b
            
        // LBP1 only has telemetry messages up until E_TELEMETRY_EVENT_DLC_OWNED
            // Deploy only has up to E_TELEMETRY_EVENT_LEAVE_LEVEL, but why is anyone using deploy
        // LBP2 has telemetry messages up until E_TELEMETRY_DCDS_ACTION
        // LBP3 has whatever is after, I'm honestly not bothering to go through them right now.
        // LBP Vita has telemetry messages up until E_TELEMETRY_GAME_PROGRESSION

        TelemetryHeader header = new();
        ushort revision = reader.ReadUInt16();

        header.Revision = revision;
        header.HashedPlayerId = reader.ReadUInt32();
        
        if (revision >= 0x12)
            reader.ReadExactly(header.LevelHash);
        
        if (revision >= 0x13)
        {
            header.SlotType = reader.ReadUInt32();
            header.SlotNumber = reader.ReadUInt32();
        }

        // All position messages have a CHash serialized before the
        // frame timestamp specifically between these two revisions and I don't
        // want to handle that case, all updated and beta builds currently in use
        // do not use these revisions, so I don't consider it a priority.
        if (revision is >= 0x10 and < 0x12)
            return BadRequest;
        
        // Between revisions 1 and 5, only the first 4 bytes of hashes were serialized
        // after these revisions, the full SHA1 is serialized.
        bool hasFullHash = revision >= 0x5;
        
        // Many messages have frame timestamps prepended after a certain revision.
        bool hasTimestamps = revision >= 0x1d;
        
        // Keep reading telemetry events until we reach the end of the stream
        // When the data is no longer bit aligned, we might read too much data,
        // so just check that we have at least 8 bits left.
        while (reader.BitsRemaining >= 8)
        {
            // Telemetry events don't include size fields, so just have to
            // parse everything, it's kind of rough.
            TelemetryEvent evt = (TelemetryEvent)reader.ReadUInt32();

            // These two events don't send any data with them, want to avoid any additional
            // conditionals for the frame timestamps added in LBP2
            if (evt is TelemetryEvent.MoveTutorial or TelemetryEvent.MoveCalibration) continue;
            
            uint frame = hasTimestamps ? reader.ReadUInt32() : 0;
            
            context.Logger.LogDebug(BunkumCategory.Game, $"{evt} from {user.Username}");
            
            switch (evt)
            {
                case TelemetryEvent.Start:
                {
                    // This doesn't send any data in early versions of LBP1
                    if (revision >= 0xd)
                    {
                        InlinePhysicalAddress addr = new();
                        reader.ReadExactly(addr);
                        
                        context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} has started sending telemetry data. MAC: {Convert.ToHexString(addr)}");
                    }
                    
                    break;
                }
                case TelemetryEvent.TestInt:
                {
                    uint num = reader.ReadUInt32();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} send test integer of value '{num}'");
                    
                    break;
                }
                case TelemetryEvent.TestV2:
                {
                    // The v2 struct in LBP is 4 components for alignment reasons,
                    // but is often used with 3 components, wonder how that works.
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} send test vector of value <{x}, {y}, {z}>");
                    
                    break;
                }
                case TelemetryEvent.TestChar:
                {
                    byte c = reader.ReadByte();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} send test character of value <{c}>");
                    
                    break;
                }
                case TelemetryEvent.TestM44:
                {
                    // I don't feel like formatting the output for a testing value honestly.
                    for (int col = 0; col < 4; ++col)
                    for (int row = 0; row < 4; ++row)
                        reader.ReadSingle();

                    break;
                }
                case TelemetryEvent.CostumesWorn:
                {
                    uint count = reader.ReadUInt32();
                    for (int i = 0; i < count; ++i)
                    {
                        if (hasTimestamps)
                        {
                            uint frameWorn = reader.ReadUInt32();   
                        }
                        
                        string costume = reader.ReadString(); // max size is 32 bytes
                        
                        context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} has worn {costume}");
                    }
                    
                    break;
                }

                case TelemetryEvent.UpdatePosition:
                case TelemetryEvent.DeathPosition:
                case TelemetryEvent.SuicidePosition:
                case TelemetryEvent.RestartPosition:
                case TelemetryEvent.QuitPosition:
                case TelemetryEvent.SwitchToEasyPosition:
                case TelemetryEvent.OffScreenPosition:
                case TelemetryEvent.PhotoPosition:
                case TelemetryEvent.LostAllLivesPosition:
                case TelemetryEvent.StickerPosition:
                case TelemetryEvent.PadDisconnectPosition:
                case TelemetryEvent.AiDeathPosition:
                case TelemetryEvent.OffscreenDeathPosition:
                {
                    TelemetryPosition pos = new()
                    {
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),
                        Layer = reader.ReadUInt32(),
                    };
                    
                    // They already added the frame to most telemetry messages,
                    // couldn't they have removed these duplicates?
                    // Seems to always be the same as the prior frame value.
                    if (revision >= 0x19)
                        pos.Frame = reader.ReadUInt32();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} - {evt} - <{pos.X}, {pos.Y}, {pos.Layer}> @ {pos.Frame}");
                    
                    break;
                }

                case TelemetryEvent.GameMessage:
                {
                    if (revision >= 0x14)
                    {
                        TelemetryGameMessage msg = new()
                        {
                            // Probably important to note that the types get moved around depending on the version of the game,
                            // for example EGMT_ALERT in LBP2 is 19, while in LBP3, it's 20
                            Type = reader.ReadUInt32(),
                        };
                        
                        // Some removed value, no builds seem to have this revision,
                        // so it's probably not important to consider.
                        if (revision < 0x15) reader.ReadUInt32();
                        else msg.Key = reader.ReadUInt32();

                        // This message has a max size of 40 bytes including the null terminator.
                        msg.Message = reader.ReadString();
                        
                        context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} has game message [type]={msg.Type}, [key]={msg.Key}, [text]={msg.Message}");
                    }
                    
                    break;
                }
                
                case TelemetryEvent.PoppetState:
                {
                    if (revision >= 0x14)
                    {
                        TelemetryPoppetState poppet = new()
                        {
                            Mode = reader.ReadUInt32(),
                            SubMode = reader.ReadUInt32(),
                        };

                        // Max size is 256 characters for whatever reason,
                        // might contain other data in certain sub modes?
                        if (revision >= 0x1d)
                            poppet.Player = reader.ReadString();
                        
                        context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} has poppet state [mode]={(PoppetMode)poppet.Mode}, [submode]={(PoppetSubMode)poppet.SubMode}, [player]={poppet.Player}");
                    }
                    
                    break;
                }
                
                case TelemetryEvent.PodComputerState:
                {
                    if (revision >= 0x14)
                    {
                        // This is just the name of the Pod Computer state returned from PodComputerState::GetName 
                        string state = reader.ReadString(); // Max string length is 512
                    
                        context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} visited pod computer state '{state}' at {frame}");   
                    }
                    
                    break;
                }
                case TelemetryEvent.ExpressionState:
                {
                    if (revision < 0x14) break;
                    
                    // 0 = HAPPY, 1 = SAD, 2 = ANGRY, 3 = SCARED, 4 = NEUTRAL
                    // The neutral message doesn't always seem to get sent?
                    // Although it might just be transitional issues.
                    uint expressionIndex = reader.ReadUInt32();
                    // Intensity of each expression, 0 means NEUTRAL,
                    uint expressionLevel = reader.ReadUInt32(); // happy
                    
                    // Not sure what this value is meant to be,
                    // it always seems to be 0.
                    int _ = reader.ReadInt32();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} has expression data [index]={expressionIndex}, [level]={expressionLevel}");
                    
                    break;
                }

                case TelemetryEvent.UserExperienceMetrics:
                {
                    if (revision < 0x17) break;
                    
                    // These values are probably not accurate in terms of names,
                    // well they could be close, since it seems they're probably(?)
                    // the same as the LBP3 JSON versions, but who knows, it at least
                    // is the correct data size.
                    TelemetryUserExperienceMetrics metrics = new()
                    {
                        CurrentMspf = reader.ReadSingle(),
                        AverageMspf = reader.ReadSingle(),
                        HighMspf = reader.ReadSingle(),
                        PredictApplied = reader.ReadUInt32(),
                        PredictDesired = reader.ReadUInt32(),
                        IsHost = reader.ReadBit(),
                        IsCreate = reader.ReadBit(),
                        NumPlayers = reader.ReadUInt32(),
                        NumPs3s = reader.ReadUInt32(),
                        AverageRttHost = reader.ReadSingle(),
                        BandwidthUsage = reader.ReadSingle(),
                        WorstPing = reader.ReadSingle(),
                        WorstBandwidth = reader.ReadSingle(),
                        WorstPacketLoss = reader.ReadSingle(),
                        WorstPlayers = reader.ReadUInt32(),
                        HttpBandwidthUp = reader.ReadSingle(),
                        HttpBandwidthDown = reader.ReadSingle(),
                        Frame = reader.ReadUInt32(),
                        LastMgjFrame = reader.ReadUInt32(),
                    };
                    
                    for (int i = 0; i < metrics.NumPlayers; ++i)
                    {
                        TelemetryPlayerNetStats stats = new()
                        {
                            Frame = reader.ReadUInt32(),
                            Player = reader.ReadUInt32(),
                            IsLocal = reader.ReadBit(),
                            AvailableBandwidth = reader.ReadUInt32(),
                            AvailableRnpBandwidth = reader.ReadUInt32(),
                            AvailableGameBandwidth = reader.ReadSingle(),
                            RecentTotalBandwidthUsed = reader.ReadUInt32(),
                            TimeBetweenSends = reader.ReadSingle(),
                        };
                        
                        metrics.PlayerNetStats.Add(stats);
                    }
                    
                    break;
                }

                case TelemetryEvent.InventoryItemClick:
                {
                    if (revision < 0x19) break;

                    TelemetryInventoryItem item = new()
                    {
                        Action = reader.ReadUInt32(),
                        Type = reader.ReadUInt32(),
                    };

                    uint numGuids = reader.ReadUInt32();
                    for (int i = 0; i < numGuids; ++i)
                        item.Guids.Add(reader.ReadUInt32());
                    uint numHashes = reader.ReadUInt32();
                    for (int i = 0; i < numHashes; ++i)
                    {
                        InlineHash hash = new();
                        reader.ReadExactly(hash);
                        item.Hashes.Add(hash);
                    }
                    
                    break;
                }

                case TelemetryEvent.OpenPsid:
                {
                    if (revision >= 0x19)
                    {
                        OpenPsid _ = new()
                        {
                            Low = reader.ReadUInt64(),
                            High = reader.ReadUInt64(),
                        };
                    }
                    
                    break;
                }

                case TelemetryEvent.Is50HzTv:
                case TelemetryEvent.IsStandardDefTv:
                case TelemetryEvent.UsingImportedLbp1Profile:
                {
                    bool _ = reader.ReadUInt32() != 0;
                    
                    break;
                }
                
                case TelemetryEvent.ImportProfile:
                {
                    if (revision >= 0x19)
                    {
                        reader.ReadUInt32();
                        reader.ReadUInt32();
                    }
                    
                    break;
                }
                
                case TelemetryEvent.ModalOverlayState:
                {
                    // This is just the name of the modal overlay state returned from ModalOverlayState::GetName
                    string state = reader.ReadString();
                    
                    context.Logger.LogDebug(BunkumCategory.Game, $"{user.Username} visited modal overlay state '{state}' at {frame}");
                    
                    break;
                }

                case TelemetryEvent.GameProgression:
                {
                    if (revision >= 0x1a)
                    {
                        uint a = reader.ReadUInt32();
                        uint b = reader.ReadUInt32();
                    }
                    
                    break;
                }

                case TelemetryEvent.MainPlayerCostume:
                {
                    uint numCostumePieces = reader.ReadUInt32();
                    for (int i = 0; i < numCostumePieces; ++i)
                        reader.ReadUInt32(); // Costume piece GUID

                    break;
                }
                    
                default:
                {
                    context.Logger.LogDebug(BunkumCategory.Game, $"Unsupported telemetry message type: {evt}");
                    
                    // Early return a 200 because why not
                    return OK;
                }
            }
        }
        
        return OK;
    }

    [GameEndpoint("t2", HttpMethods.Post, ContentType.Json)]
    [MinimumRole(GameUserRole.Restricted)]
    [DebugRequestBody]
    public Response UploadJsonTelemetry(RequestContext context, JsonTelemetryEvents body, GameUser user, MatchService match)
    {
        foreach (JsonTelemetryEvent telemetryEvent in body.Events)
        {
            if (!Enum.TryParse((string)telemetryEvent.Data["event_type"]!, out JsonTelemetryEventType eventType))
            {
                context.Logger.LogWarning(
                    RefreshContext.Telemetry,
                    "Unhandled telemetry type {0}, data: {1}, custom data: {2}",
                    telemetryEvent.Data["event_type"]!,
                    telemetryEvent.Data.ToString(Formatting.None),
                    telemetryEvent.CustomData?.ToString(Formatting.None) ?? "null"
                );

                continue;
            }

            switch (eventType)
            {
                case JsonTelemetryEventType.UserXpm:
                {
                    TelemetryUserXpmEvent data = telemetryEvent.Data.ToObject<TelemetryUserXpmEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got user experience telemetry event, current fps: {0}, avg fps: {1}", 1000.0 / data.CurMspf,
                        1000.0 / data.AvgMspf);
                    break;
                }
                case JsonTelemetryEventType.NetworkProfile:
                {
                    TelemetryNetworkProfileEvent data = telemetryEvent.Data.ToObject<TelemetryNetworkProfileEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got network profile event, nat type: {0}",
                        data.NatType);
                    break;
                }
                case JsonTelemetryEventType.UserProfile:
                {
                    TelemetryUserProfileEvent data = telemetryEvent.Data.ToObject<TelemetryUserProfileEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got user profile event, online ID: {0}, account ID: {1}", data.NpOnlineId, data.NpAccountId);
                    break;
                }
                case JsonTelemetryEventType.FriendProfile:
                {
                    TelemetryFriendProfileEvent data = telemetryEvent.Data.ToObject<TelemetryFriendProfileEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got friend profile event, friends: {0}, blocked: {1}", string.Join(',', data.FriendList),
                        string.Join(',', data.BlockList));
                    break;
                }
                case JsonTelemetryEventType.DLCProfile:
                {
                    TelemetryDlcProfileEvent data = telemetryEvent.Data.ToObject<TelemetryDlcProfileEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got dlc profile event, sku: {0}",
                        string.Join(',', data.Sku));
                    break;
                }
                case JsonTelemetryEventType.GameStart:
                {
                    TelemetryGameStartEvent data = telemetryEvent.Data.ToObject<TelemetryGameStartEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got game start event, level id: {0}",
                        data.LevelId);
                    break;
                }
                case JsonTelemetryEventType.GameEnd:
                {
                    TelemetryGameEndEvent data = telemetryEvent.Data.ToObject<TelemetryGameEndEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got game end event, level id: {0}", data.LevelId);
                    break;
                }
                case JsonTelemetryEventType.PlayerJoin:
                {
                    TelemetryPlayerJoinEvent data = telemetryEvent.Data.ToObject<TelemetryPlayerJoinEvent>()!;
                    TelemetryPlayerJoinEventCustomData? customData =
                        telemetryEvent.CustomData?.ToObject<TelemetryPlayerJoinEventCustomData>();
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got player join event, online id: {0}, how matched: {1}", data.NpOnlineId,
                        customData?.HowMatched ?? TelemetryPlayerJoinHowMatched.Unknown);
                    break;
                }
                case JsonTelemetryEventType.PlayerLeave:
                {
                    TelemetryPlayerLeaveEvent data = telemetryEvent.Data.ToObject<TelemetryPlayerLeaveEvent>()!;
                    TelemetryPlayerLeaveEventCustomData? customData =
                        telemetryEvent.CustomData?.ToObject<TelemetryPlayerLeaveEventCustomData>();
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got player leave event, online id: {0}, game time: {1}", data.NpOnlineId,
                        customData?.GameTime ?? 0);
                    break;
                }
                case JsonTelemetryEventType.ResourceError:
                {
                    TelemetryResourceErrorEvent data = telemetryEvent.Data.ToObject<TelemetryResourceErrorEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got resource error event, hash: {0}, guid: {1}",
                        data.Hash, data.Guid);
                    break;
                }
                case JsonTelemetryEventType.ChkPtHit:
                {
                    TelemetryCheckpointHitEvent data = telemetryEvent.Data.ToObject<TelemetryCheckpointHitEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got checkpoint hit event, respawn: {0}, checkpoint id: {1}", data.Respawn, data.CheckpointId);
                    break;
                }
                case JsonTelemetryEventType.PinAchieve:
                {
                    TelemetryPinAchieveEvent data = telemetryEvent.Data.ToObject<TelemetryPinAchieveEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got pin achieve event, type: {0}, id: {1}",
                        data.PinType, data.PinId);
                    break;
                }
                case JsonTelemetryEventType.QuestAdded:
                {
                    TelemetryQuestAddedEvent data = telemetryEvent.Data.ToObject<TelemetryQuestAddedEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got quest added event, name: {0}",
                        data.QuestName);
                    break;
                }
                case JsonTelemetryEventType.HeatmapPos:
                {
                    TelemetryHeatmapPosEvent data = telemetryEvent.Data.ToObject<TelemetryHeatmapPosEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got quest added event, reason: {0}, pos:{1},{2},{3}", data.Reason, data.X, data.Y, data.Z);
                    break;
                }
                case JsonTelemetryEventType.TutorVid:
                {
                    TelemetryTutorVidEvent data = telemetryEvent.Data.ToObject<TelemetryTutorVidEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got tutorial video event, video: {0}, title: {1}",
                        data.Video, data.Title);
                    break;
                }
                case JsonTelemetryEventType.LevelHacked:
                {
                    TelemetryLevelHackedEvent data = telemetryEvent.Data.ToObject<TelemetryLevelHackedEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got level hacked event, level id: {0}",
                        data.LevelId);
                    break;
                }
                case JsonTelemetryEventType.PlayerScore:
                {
                    TelemetryPlayerScoreEvent data = telemetryEvent.Data.ToObject<TelemetryPlayerScoreEvent>()!;
                    TelemetryPlayerScoreEventCustomData customData =
                        telemetryEvent.Data.ToObject<TelemetryPlayerScoreEventCustomData>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got player score event, score: {0}, highest mult: {1}, deaths: {2}", data.Score,
                        customData.HighestMult, customData.Deaths);
                    break;
                }
                case JsonTelemetryEventType.CharacterType:
                {
                    TelemetryCharacterTypeEvent data = telemetryEvent.Data.ToObject<TelemetryCharacterTypeEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got character type event, character: {0}",
                        data.PlayerCharacter);
                    break;
                }
                case JsonTelemetryEventType.ModalOS:
                {
                    TelemetryModalOsEvent data = telemetryEvent.Data.ToObject<TelemetryModalOsEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got modal os event, modal state: {0}",
                        data.ModalState);
                    break;
                }
                case JsonTelemetryEventType.PublishDLC:
                {
                    TelemetryPublishDlcEvent data = telemetryEvent.Data.ToObject<TelemetryPublishDlcEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got publish dlc event, action taken: {0}",
                        data.ActionTaken);
                    break;
                }
                case JsonTelemetryEventType.AnimationSelected:
                {
                    TelemetryAnimationSelectedEvent data =
                        telemetryEvent.Data.ToObject<TelemetryAnimationSelectedEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got animation selected event, character: {0}, guid: {1}", data.Character, data.AnimationGuid);
                    break;
                }
                case JsonTelemetryEventType.InvClick:
                {
                    TelemetryInventoryClickEvent data = telemetryEvent.Data.ToObject<TelemetryInventoryClickEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got inventory click event, action: {0}, type: {1}, hashes: {2}, guids: {3}", data.Action,
                        data.Type, string.Join(',', data.Hashes), string.Join(',', data.Guids));
                    break;
                }
                case JsonTelemetryEventType.NetworkJoinResult:
                {
                    TelemetryNetworkJoinResultEvent data =
                        telemetryEvent.Data.ToObject<TelemetryNetworkJoinResultEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got network join result event, response: {0}",
                        data.Response);
                    break;
                }
                case JsonTelemetryEventType.BootStart:
                {
                    TelemetryBootStartEvent data = telemetryEvent.Data.ToObject<TelemetryBootStartEvent>()!;
                    TelemetryBootStartEventCustomData? customData =
                        telemetryEvent.CustomData?.ToObject<TelemetryBootStartEventCustomData>();
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got boot start event, build version: {0}",
                        data.BuildVersion);
                    break;
                }
                case JsonTelemetryEventType.HardwareProfile:
                {
                    TelemetryHardwareProfileEvent data = telemetryEvent.Data.ToObject<TelemetryHardwareProfileEvent>()!;
                    TelemetryHardwareProfileEventCustomData customData =
                        telemetryEvent.CustomData!.ToObject<TelemetryHardwareProfileEventCustomData>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got hardware profile event, language setting: {0}, camera type: {1}, headset type: {2}, console: {3}",
                        data.LanguageSetting, data.CameraType, data.HeadsetType,
                        customData?.Console ?? TelemetryHardwareProfileConsole.Ps3);
                    break;
                }
                case JsonTelemetryEventType.MenuScreen:
                {
                    TelemetryMenuScreenEvent data = telemetryEvent.Data.ToObject<TelemetryMenuScreenEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got menu screen event, menu screen: {0}",
                        data.MenuScreen);
                    break;
                }
                case JsonTelemetryEventType.CommunityUI:
                {
                    TelemetryCommunityUiEvent data = telemetryEvent.Data.ToObject<TelemetryCommunityUiEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got community ui event, action: {0}",
                        data.Action);
                    break;
                }
                case JsonTelemetryEventType.AddToCart:
                case JsonTelemetryEventType.RemoveFromCart:
                case JsonTelemetryEventType.ItemDetailView:
                {
                    TelemetryCartInteractionEvent data = telemetryEvent.Data.ToObject<TelemetryCartInteractionEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got {0} event, sku: {1}", eventType, data.Sku);
                    break;
                }
                case JsonTelemetryEventType.DiveInEvent:
                {
                    TelemetryDiveInEventEvent data = telemetryEvent.Data.ToObject<TelemetryDiveInEventEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got dive in event event, event: {0}", data.Event);
                    break;
                }
                case JsonTelemetryEventType.Error:
                {
                    TelemetryErrorEvent data = telemetryEvent.Data.ToObject<TelemetryErrorEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got error event, error: {0}, code: {1}, message: {2}", data.ErrorType, data.ErrorCode,
                        data.ErrorMessage);
                    break;
                }
                case JsonTelemetryEventType.GenericMessage:
                {
                    TelemetryGenericMessageEvent data = telemetryEvent.Data.ToObject<TelemetryGenericMessageEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got generic message event, message: {0}",
                        data.Message);
                    break;
                }
                case JsonTelemetryEventType.ImportPhoto:
                {
                    TelemetryImportPhotoEvent data = telemetryEvent.Data.ToObject<TelemetryImportPhotoEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got import photo event, uid: {0}, hash: {1}, filename: {2}", data.Uid, data.Hash, data.Fname);
                    break;
                }
                case JsonTelemetryEventType.LiveStreamStart: {
                    TelemetryLiveStreamStartEvent data = telemetryEvent.Data.ToObject<TelemetryLiveStreamStartEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got live stream start event, name: {0}, stream service: {1}, id: {2}, interactive: {3}, command words: {4}",
                        data.Name, data.StreamService, data.StreamId, data.Interactive ?? false,
                        string.Join(',', data.InteractiveCommandWords ?? []));
                    break;
                }
                case JsonTelemetryEventType.LiveStreamUpdate: {
                    TelemetryLiveStreamUpdateEvent data = telemetryEvent.Data.ToObject<TelemetryLiveStreamUpdateEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got live stream start event, current viewers: {0}, id: {1}, interactive: {2}, command words: {3}",
                        data.CurrentViewers, data.StreamId, data.Interactive ?? false,
                        string.Join(',', data.InteractiveCommandWords ?? []));
                    break;
                }
                case JsonTelemetryEventType.LiveStreamEnd: {
                    TelemetryLiveStreamEndEvent data = telemetryEvent.Data.ToObject<TelemetryLiveStreamEndEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got live stream start event, current viewers: {0}, id: {1}, peak viewers: {2}, stream length: {3} secs",
                        data.CurrentViewers, data.StreamId, data.PeakViewers, data.StreamLengthSecs);
                    break;
                }
                case JsonTelemetryEventType.NetworkResourceError:
                {
                    TelemetryNetworkResourceErrorEvent data = telemetryEvent.Data.ToObject<TelemetryNetworkResourceErrorEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got network resource error event, guid: {0}, hash: {1}, error type: {2}",
                        data.Guid, data.Hash, data.NetworkErrorType);
                    break;
                }
                case JsonTelemetryEventType.NetworkTimeout:
                {
                    TelemetryNetworkTimeoutEvent data = telemetryEvent.Data.ToObject<TelemetryNetworkTimeoutEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got network timeout event, client id: {0}, host id: {1}, error: {2}",
                        data.ClientId, data.HostId, data.Error);
                    break;
                }
                case JsonTelemetryEventType.ObjectiveComplete:
                {
                    TelemetryObjectiveCompleteEvent data = telemetryEvent.Data.ToObject<TelemetryObjectiveCompleteEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got objective complete event, quest: {0}, name: {1}, id: {2}",
                        data.Quest, data.ObjctiveName, data.ObjectiveId);
                    break;
                }
                case JsonTelemetryEventType.Ping:
                {
                    TelemetryPingEvent data = telemetryEvent.Data.ToObject<TelemetryPingEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got ping event, user: {0}, packet loss: {1}, bandwidth available: {2}kbps, bandwidth used: {3}kbps, bandwidth np available: {4}kbps",
                        data.User, data.PacketLoss, data.RoundTripDelay, data.BandwidthAvailableKbps, data.BandwidthUsedKbps, data.BandwidthNetplayAvailableKbps);
                    break;
                }
                case JsonTelemetryEventType.PlayNow:        
                {
                    TelemetryPlayNowEvent data = telemetryEvent.Data.ToObject<TelemetryPlayNowEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got play now event, slot: [{0}, {1}], source: {2}",
                        data.Slot[0], data.Slot[1], data.Source);
                    break;
                }
                case JsonTelemetryEventType.QuestCompleted:
                {
                    TelemetryQuestCompletedEvent data = telemetryEvent.Data.ToObject<TelemetryQuestCompletedEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got quest completed event, slot: [{0}, {1}], id: {2}, name: {3}",
                        data.Slot[0], data.Slot[1], data.QuestId, data.QuestName);
                    break;
                }
                case JsonTelemetryEventType.CommunitySearch:
                {
                    TelemetryCommunitySearchEvent data = telemetryEvent.Data.ToObject<TelemetryCommunitySearchEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got community search event, text: {0}",
                        data.Text);
                    break;
                }
                case JsonTelemetryEventType.StoreSearch:
                {
                    TelemetryStoreSearchEvent data = telemetryEvent.Data.ToObject<TelemetryStoreSearchEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got store search event, text: {0}",
                        data.Text);
                    break;
                }
                case JsonTelemetryEventType.SocialInteraction:
                {
                    TelemetrySocialInteractionEvent data = telemetryEvent.Data.ToObject<TelemetrySocialInteractionEvent>()!;
                    TelemetrySocialInteractionEventCustomData customData = telemetryEvent.CustomData!.ToObject<TelemetrySocialInteractionEventCustomData>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got social interaction event, type: {0}, target: {1}, length: {2}",
                        data.InteractionType, customData.Target, customData.Length);
                    break;
                }
                case JsonTelemetryEventType.TrophyAchieve:
                {
                    TelemetryTrophyAchieveEvent data = telemetryEvent.Data.ToObject<TelemetryTrophyAchieveEvent>()!;
                    TelemetryTrophyAchieveEventCustomData customData = telemetryEvent.CustomData!.ToObject<TelemetryTrophyAchieveEventCustomData>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry,
                        "Got trophy achieve event, trophy id: {0}, trophy type: {1}, is first time: {2}, set version: {3}, localized name: {4}",
                        data.TrophyId, data.TrophyType, data.IsFirstTime, data.TrophySetVersion, customData.Localized);
                    break;
                }
                case JsonTelemetryEventType.SoundSelected:
                {
                    TelemetrySoundSelectedEvent data = telemetryEvent.Data.ToObject<TelemetrySoundSelectedEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got sound selected event, name: {0}",
                        data.Name);
                    break;
                }
                case JsonTelemetryEventType.StoreEnd:
                {
                    TelemetryStoreEndEvent data = telemetryEvent.Data.ToObject<TelemetryStoreEndEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got store end event, session id: {0}, duration secs: {1}, did purchase: {2}",
                        data.StoreSessionId, data.DurationSecs, data.DidPurchase);
                    break;
                }
                case JsonTelemetryEventType.StoreMenuScreen:
                {
                    TelemetryStoreMenuScreenEvent data = telemetryEvent.Data.ToObject<TelemetryStoreMenuScreenEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got store menu screen event, session id: {0}, screen id: {1}",
                        data.StoreSessionId, data.ScreenId);
                    break;
                }
                case JsonTelemetryEventType.StoreStart:
                {
                    TelemetryStoreStartEvent data = telemetryEvent.Data.ToObject<TelemetryStoreStartEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got store start event, session id: {0}, entry point: {1}",
                        data.StoreSessionId, data.EntryPoint);
                    break;
                }
                case JsonTelemetryEventType.TkPhoto:
                {
                    TelemetryTakePhotoEvent data = telemetryEvent.Data.ToObject<TelemetryTakePhotoEvent>()!;
                    context.Logger.LogInfo(RefreshContext.Telemetry, "Got take photo event, action: {0}, game id: {1}",
                        data.Action, data.GameId);
                    break;
                }
                case JsonTelemetryEventType.Hearted:
                {
                    TelemetryGenericHeartedEvent genericData =
                        telemetryEvent.Data.ToObject<TelemetryGenericHeartedEvent>()!;
                    switch (genericData.Type)
                    {
                        case TelemetryHeartedType.Level:
                        case TelemetryHeartedType.Adventure:
                        {
                            TelemetrySlotHeartedEventData data =
                                telemetryEvent.Data.ToObject<TelemetrySlotHeartedEventData>()!;

                            context.Logger.LogInfo(RefreshContext.Telemetry,
                                "Got slot hearted event ({5}), hearted: {0}, level: [{1}, {2}], level owner: {3}, meta: {4}",
                                data.Hearted, data.Level[0], data.Level[1], data.LevelOwner, data.Meta, genericData.Type);
                            break;
                        }
                        case TelemetryHeartedType.User:
                        {
                            TelemetryUserHeartedEventData data =
                                telemetryEvent.Data.ToObject<TelemetryUserHeartedEventData>()!;
                            
                            context.Logger.LogInfo(RefreshContext.Telemetry,
                                "Got user hearted event, hearted: {0}, meta: {1}",
                                data.Hearted, data.Meta);
                            break;
                        }
                        case TelemetryHeartedType.Playlist:
                        {
                            TelemetryPlaylistHeartedEventData data =
                                telemetryEvent.Data.ToObject<TelemetryPlaylistHeartedEventData>()!;

                            context.Logger.LogInfo(RefreshContext.Telemetry,
                                "Got playlist hearted event, hearted: {0}, playlist: {1}",
                                data.Hearted, data.Playlist);
                            break;
                        }
                        case TelemetryHeartedType.Item:
                        {
                            TelemetryItemHeartedEventData data =
                                telemetryEvent.Data.ToObject<TelemetryItemHeartedEventData>()!;
                            
                            context.Logger.LogInfo(RefreshContext.Telemetry,
                                "Got item hearted event, hearted: {0}, uid: {1}, guid: {2}",
                                data.Hearted, data.Uid, data.Guid);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return OK;
    }
}