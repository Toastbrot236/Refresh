using System.Xml.Serialization;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Endpoints.Debugging;
using Bunkum.Core.RateLimit;
using Bunkum.Core.Responses;
using Bunkum.Core.Responses.Serialization;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.Common.Time;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Configuration;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.Game.Types.Challenges.Lbp3;
using Refresh.Interfaces.Game.Types.UserData;

namespace Refresh.Interfaces.Game.Endpoints.Handshake;

public class MetadataEndpoints : EndpointGroup
{
    private const int ConfigRequestTimeoutDuration = 480;
    private const int ConfigRequestAmount = 40;
    private const int ConfigBlockDuration = 420;
    private const string ConfigBucket = "game-configs";

    [GameEndpoint("privacySettings", ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    public SerializedPrivacySettings GetPrivacySettings(RequestContext context, GameUser user)
    {
        return new SerializedPrivacySettings
        {
            LevelVisibility = user.LevelVisibility,
            ProfileVisibility = user.ProfileVisibility,
        };
    }
    
    [GameEndpoint("privacySettings", ContentType.Xml, HttpMethods.Post)]
    [MinimumRole(GameUserRole.Restricted)]
    public SerializedPrivacySettings SetPrivacySettings(RequestContext context, SerializedPrivacySettings body, GameDatabaseContext database, GameUser user)
    {
        database.SetPrivacySettings(user, body);
        
        return body;
    }

    [GameEndpoint("npdata", ContentType.Xml, HttpMethods.Post)]
    [RateLimitSettings(480, 8, 420, "game-npdata")]
    public Response SetFriendData(RequestContext context, GameUser user, GameDatabaseContext database, SerializedFriendData body, DataContext dataContext, GameServerConfig config)
    {
        if (user.IsWriteBlocked(config))
            return OK; // else the game will send a new request for this every minute

        IEnumerable<GameUser> friends = body.FriendsList.Names
            .Take(128) // should be way more than enough - we'll see if this becomes a problem
            .Select(username => database.GetUserByUsername(username))
            .Where(u => u != null)!;
        
        foreach (GameUser userToFavourite in friends)
        {
            database.FavouriteUser(userToFavourite, user);
            dataContext.Cache.RemoveOwnUserRelations(user, userToFavourite); // really don't want to refresh the relations of up to 128 users in the worst case
        }
        
        return OK;
    }

    private static readonly Lazy<string?> NetworkSettingsFile
        = new(() => 
        {
            string path = Path.Combine(Environment.CurrentDirectory, "network_settings.nws");

            return File.Exists(path) ? File.ReadAllText(path) : null;
        });
    
    [GameEndpoint("network_settings.nws")]
    [MinimumRole(GameUserRole.Restricted)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string NetworkSettings(RequestContext context, GameServerConfig config)
    {
        string? networkSettings = NetworkSettingsFile.Value;

        // EnableHackChecks being false fixes the "There was a problem with the level you were playing on that forced a return to your Pod." error that LBP3 tends to show in the pod.
        // AlexDB
        //  - Enables the "Web Privacy Settings" option on LBP1
        //  - Enables in-game queuing on LBP1
        //  - Part of the check for enabling LBP1 Playlists
        //  - Adds "Mm Picks" and "Lucky Dip" search options on LBP1
        // OverheatingThreshholdDisallowMidgameJoin is set to >1.0 so that it never triggers
        // ShowLevelBoos, EnableCommunityDecorations, EnablePlayedFilter enable various game features
        // DisableDLCPublishCheck disables the game's DLC publish check.
        // SECONDS_BETWEEN_PINS_AWARDED_UPLOADS is forced to the value which both LBP2 and 3 default to (5 minutes) incase a beta build sets a different value.
        networkSettings ??= $"""
                            AllowOnlineCreate true
                            ShowErrorNumbers true
                            AllowModeratedLevels false
                            AllowModeratedPoppetItems false
                            ShowLevelBoos true
                            CDNHostName {config.GameConfigStorageUrl}
                            TelemetryServer {config.GameConfigStorageUrl}
                            OverheatingThresholdDisallowMidgameJoin 2.0
                            EnableCommunityDecorations true
                            EnablePlayedFilter true
                            EnableDiveIn {(config.EnableDiveIn ? "true" : "false")}
                            EnableHackChecks false
                            DisableDLCPublishCheck true
                            AlexDB true
                            SECONDS_BETWEEN_PINS_AWARDED_UPLOADS 300.0
                            """;
        
        return networkSettings;
    }
    
    private static readonly Lazy<string?> TelemetryConfigFile
        = new(() => 
        {
            string path = Path.Combine(Environment.CurrentDirectory, "telemetry.xml");

            return File.Exists(path) ? File.ReadAllText(path) : null;
        });

    [GameEndpoint("t_conf")]
    [GameEndpoint("telemetry.cfg")]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(Gone)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string? TelemetryConfig(RequestContext context)
    {
        bool created = TelemetryConfigFile.IsValueCreated;

        string? telemetryConfig = TelemetryConfigFile.Value;

        // Only log this warning once
        if (!created && telemetryConfig == null)
            context.Logger.LogWarning(BunkumCategory.Request, "telemetry.xml file is missing! " +
                                                              "LBP will work without it, but it may be relevant to you if you are an advanced user.");

        string a = """
                   <t_enable>true</t_enable>
                   <t_bandwidth_cap>32000.0</t_bandwidth_cap>
                   <t_secs_between_flush_buffer>5.0</t_secs_between_flush_buffer>
                   <t_version>1</t_version>
                   <t_max_dlc_to_report>32</t_max_dlc_to_report>

                   <t_percentages>
                       <E_TELEMETRY_EVENT_DEBUG_NAME_LOOKUP>100.0</E_TELEMETRY_EVENT_DEBUG_NAME_LOOKUP>
                       <E_TELEMETRY_EVENT_MEMORY_STATS>100.0</E_TELEMETRY_EVENT_MEMORY_STATS>
                       <E_TELEMETRY_EVENT_BOOT_START>100.0</E_TELEMETRY_EVENT_BOOT_START>
                       <E_TELEMETRY_EVENT_PLAYER_PROFILE>100.0</E_TELEMETRY_EVENT_PLAYER_PROFILE>
                       <E_TELEMETRY_EVENT_FRIEND_PROFILE>100.0</E_TELEMETRY_EVENT_FRIEND_PROFILE>
                       <E_TELEMETRY_EVENT_HARDWARE_PROFILE>100.0</E_TELEMETRY_EVENT_HARDWARE_PROFILE>
                       <E_TELEMETRY_EVENT_NETWORK_PROFILE>100.0</E_TELEMETRY_EVENT_NETWORK_PROFILE>
                       <E_TELEMETRY_EVENT_DLC_PROFILE>100.0</E_TELEMETRY_EVENT_DLC_PROFILE>
                       <E_TELEMETRY_EVENT_GAME_START>100.0</E_TELEMETRY_EVENT_GAME_START>
                       <E_TELEMETRY_EVENT_PLAYER_JOIN>100.0</E_TELEMETRY_EVENT_PLAYER_JOIN>
                       <E_TELEMETRY_EVENT_PLAYER_LEAVE>100.0</E_TELEMETRY_EVENT_PLAYER_LEAVE>
                       <E_TELEMETRY_EVENT_GAME_END>100.0</E_TELEMETRY_EVENT_GAME_END>
                       <E_TELEMETRY_EVENT_PLAYER_SCORE>100.0</E_TELEMETRY_EVENT_PLAYER_SCORE>
                       <E_TELEMETRY_EVENT_STORE_START>100.0</E_TELEMETRY_EVENT_STORE_START>
                       <E_TELEMETRY_EVENT_STORE_MENU_SCREEN>100.0</E_TELEMETRY_EVENT_STORE_MENU_SCREEN>
                       <E_TELEMETRY_EVENT_ITEM_DETAIL_VIEW>100.0</E_TELEMETRY_EVENT_ITEM_DETAIL_VIEW>
                       <E_TELEMETRY_EVENT_ITEM_ADDED_TO_CART>100.0</E_TELEMETRY_EVENT_ITEM_ADDED_TO_CART>
                       <E_TELEMETRY_EVENT_ITEM_REMOVE_FROM_CART>100.0</E_TELEMETRY_EVENT_ITEM_REMOVE_FROM_CART>
                       <E_TELEMETRY_EVENT_CART_CHECKOUT_PROCESS_STARTED>100.0</E_TELEMETRY_EVENT_CART_CHECKOUT_PROCESS_STARTED>
                       <E_TELEMETRY_EVENT_CART_RESULT_DATA>100.0</E_TELEMETRY_EVENT_CART_RESULT_DATA>
                       <E_TELEMETRY_EVENT_STORE_END>100.0</E_TELEMETRY_EVENT_STORE_END>
                       <E_TELEMETRY_EVENT_ERROR>100.0</E_TELEMETRY_EVENT_ERROR>
                       <E_TELEMETRY_EVENT_TROPHY_ACHIEVE>100.0</E_TELEMETRY_EVENT_TROPHY_ACHIEVE>
                       <E_TELEMETRY_EVENT_SOCIAL_INTERACTION>100.0</E_TELEMETRY_EVENT_SOCIAL_INTERACTION>
                       <E_TELEMETRY_EVENT_MENU_SCREEN>100.0</E_TELEMETRY_EVENT_MENU_SCREEN>
                       <E_TELEMETRY_EVENT_LEVEL_PUBLISH>100.0</E_TELEMETRY_EVENT_LEVEL_PUBLISH>
                       <E_TELEMETRY_EVENT_INVENTORY_ITEM_CLICK>100.0</E_TELEMETRY_EVENT_INVENTORY_ITEM_CLICK>
                       <E_TELEMETRY_EVENT_MODAL_OVERLAY_STATE>100.0</E_TELEMETRY_EVENT_MODAL_OVERLAY_STATE>
                       <E_TELEMETRY_EVENT_TAKE_PHOTO>100.0</E_TELEMETRY_EVENT_TAKE_PHOTO>
                       <E_TELEMETRY_EVENT_ITEM_HEARTED>100.0</E_TELEMETRY_EVENT_ITEM_HEARTED>
                       <E_TELEMETRY_EVENT_DEATH_POSITION>100.0</E_TELEMETRY_EVENT_DEATH_POSITION>
                       <E_TELEMETRY_EVENT_SUICIDE_POSITION>100.0</E_TELEMETRY_EVENT_SUICIDE_POSITION>
                       <E_TELEMETRY_EVENT_RESTART_POSITION>100.0</E_TELEMETRY_EVENT_RESTART_POSITION>
                       <E_TELEMETRY_EVENT_LOST_ALL_LIVES_POSITION>100.0</E_TELEMETRY_EVENT_LOST_ALL_LIVES_POSITION>
                       <E_TELEMETRY_EVENT_QUIT_POSITION>100.0</E_TELEMETRY_EVENT_QUIT_POSITION>
                       <E_TELEMETRY_EVENT_CONNECTION_QUALITY>100.0</E_TELEMETRY_EVENT_CONNECTION_QUALITY>
                       <E_TELEMETRY_EVENT_USER_EXPERIENCE_METRICS>100.0</E_TELEMETRY_EVENT_USER_EXPERIENCE_METRICS>
                       <E_TELEMETRY_EVENT_LIVE_STREAM_STARTED>100.0</E_TELEMETRY_EVENT_LIVE_STREAM_STARTED>
                       <E_TELEMETRY_EVENT_LIVE_STREAM_UPDATE>100.0</E_TELEMETRY_EVENT_LIVE_STREAM_UPDATE>
                       <E_TELEMETRY_EVENT_LIVE_STREAM_ENDED>100.0</E_TELEMETRY_EVENT_LIVE_STREAM_ENDED>
                       <E_TELEMETRY_EVENT_PLAY_NOW>100.0</E_TELEMETRY_EVENT_PLAY_NOW>
                       <E_TELEMETRY_EVENT_QUEST_ADDED>100.0</E_TELEMETRY_EVENT_QUEST_ADDED>
                       <E_TELEMETRY_EVENT_QUEST_COMPLETED>100.0</E_TELEMETRY_EVENT_QUEST_COMPLETED>
                       <E_TELEMETRY_EVENT_OBJECTIVE_COMPLETED>100.0</E_TELEMETRY_EVENT_OBJECTIVE_COMPLETED>
                       <E_TELEMETRY_EVENT_COMMUNITY_ACTION_UI>100.0</E_TELEMETRY_EVENT_COMMUNITY_ACTION_UI>
                       <E_TELEMETRY_EVENT_TUTORIAL_VIDEO>100.0</E_TELEMETRY_EVENT_TUTORIAL_VIDEO>
                       <E_TELEMETRY_EVENT_STORE_TEXT_SEARCH>100.0</E_TELEMETRY_EVENT_STORE_TEXT_SEARCH>
                       <E_TELEMETRY_EVENT_COMMUNITY_TEXT_SEARCH>100.0</E_TELEMETRY_EVENT_COMMUNITY_TEXT_SEARCH>
                       <E_TELEMETRY_EVENT_PIN_ACHIEVE>100.0</E_TELEMETRY_EVENT_PIN_ACHIEVE>
                       <E_TELEMETRY_EVENT_PIN_ACHIEVE_FIRST_TIME>100.0</E_TELEMETRY_EVENT_PIN_ACHIEVE_FIRST_TIME>
                       <E_TELEMETRY_EVENT_IMPORT_PHOTO>100.0</E_TELEMETRY_EVENT_IMPORT_PHOTO>
                       <E_TELEMETRY_CHECKPOINT_RESPAWNED>100.0</E_TELEMETRY_CHECKPOINT_RESPAWNED>
                       <E_TELEMETRY_CHECKPOINT_HIT>100.0</E_TELEMETRY_CHECKPOINT_HIT>
                       <E_TELEMETRY_CREATURE_TRACK>100.0</E_TELEMETRY_CREATURE_TRACK>
                       <E_TELEMETRY_MISSING_RESOURCE>100.0</E_TELEMETRY_MISSING_RESOURCE>
                       <E_TELEMETRY_MISSING_NETWORK_RESOURCE>100.0</E_TELEMETRY_MISSING_NETWORK_RESOURCE>
                       <E_TELEMETRY_TRYING_TO_PUBLISH_DLC>100.0</E_TELEMETRY_TRYING_TO_PUBLISH_DLC>
                       <E_TELEMETRY_NETWORK_JOIN_RESULT>100.0</E_TELEMETRY_NETWORK_JOIN_RESULT>
                       <E_TELEMETRY_NETWORK_ERROR>100.0</E_TELEMETRY_NETWORK_ERROR>
                       <E_TELEMETRY_ANIMATION_SELECTED>100.0</E_TELEMETRY_ANIMATION_SELECTED>
                       <E_TELEMETRY_SOUND_SELECTED>100.0</E_TELEMETRY_SOUND_SELECTED>
                       <E_TELEMETRY_HACKED_LEVEL>100.0</E_TELEMETRY_HACKED_LEVEL>
                       <E_TELEMETRY_DIVE_IN_EVENT>100.0</E_TELEMETRY_DIVE_IN_EVENT>
                       <E_TELEMETRY_GENERIC_MESSAGE>100.0</E_TELEMETRY_GENERIC_MESSAGE>
                       <E_TELEMETRY_EVENT_COUNT>100.0</E_TELEMETRY_EVENT_COUNT>
                       <E_TELEMETRY_EVENT_INVALID>100.0</E_TELEMETRY_EVENT_INVALID>
                     
                   </t_percentages>

                   <t_timing>
                     <E_TELEMETRY_EVENT_DEBUG_NAME_LOOKUP>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_DEBUG_NAME_LOOKUP>
                   <E_TELEMETRY_EVENT_MEMORY_STATS>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_MEMORY_STATS>
                   <E_TELEMETRY_EVENT_BOOT_START>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_BOOT_START>
                   <E_TELEMETRY_EVENT_PLAYER_PROFILE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PLAYER_PROFILE>
                   <E_TELEMETRY_EVENT_FRIEND_PROFILE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_FRIEND_PROFILE>
                   <E_TELEMETRY_EVENT_HARDWARE_PROFILE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_HARDWARE_PROFILE>
                   <E_TELEMETRY_EVENT_NETWORK_PROFILE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_NETWORK_PROFILE>
                   <E_TELEMETRY_EVENT_DLC_PROFILE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_DLC_PROFILE>
                   <E_TELEMETRY_EVENT_GAME_START>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_GAME_START>
                   <E_TELEMETRY_EVENT_PLAYER_JOIN>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PLAYER_JOIN>
                   <E_TELEMETRY_EVENT_PLAYER_LEAVE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PLAYER_LEAVE>
                   <E_TELEMETRY_EVENT_GAME_END>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_GAME_END>
                   <E_TELEMETRY_EVENT_PLAYER_SCORE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PLAYER_SCORE>
                   <E_TELEMETRY_EVENT_STORE_START>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_STORE_START>
                   <E_TELEMETRY_EVENT_STORE_MENU_SCREEN>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_STORE_MENU_SCREEN>
                   <E_TELEMETRY_EVENT_ITEM_DETAIL_VIEW>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_ITEM_DETAIL_VIEW>
                   <E_TELEMETRY_EVENT_ITEM_ADDED_TO_CART>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_ITEM_ADDED_TO_CART>
                   <E_TELEMETRY_EVENT_ITEM_REMOVE_FROM_CART>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_ITEM_REMOVE_FROM_CART>
                   <E_TELEMETRY_EVENT_CART_CHECKOUT_PROCESS_STARTED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_CART_CHECKOUT_PROCESS_STARTED>
                   <E_TELEMETRY_EVENT_CART_RESULT_DATA>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_CART_RESULT_DATA>
                   <E_TELEMETRY_EVENT_STORE_END>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_STORE_END>
                   <E_TELEMETRY_EVENT_ERROR>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_ERROR>
                   <E_TELEMETRY_EVENT_TROPHY_ACHIEVE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_TROPHY_ACHIEVE>
                   <E_TELEMETRY_EVENT_SOCIAL_INTERACTION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_SOCIAL_INTERACTION>
                   <E_TELEMETRY_EVENT_MENU_SCREEN>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_MENU_SCREEN>
                   <E_TELEMETRY_EVENT_LEVEL_PUBLISH>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_LEVEL_PUBLISH>
                   <E_TELEMETRY_EVENT_INVENTORY_ITEM_CLICK>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_INVENTORY_ITEM_CLICK>
                   <E_TELEMETRY_EVENT_MODAL_OVERLAY_STATE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_MODAL_OVERLAY_STATE>
                   <E_TELEMETRY_EVENT_TAKE_PHOTO>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_TAKE_PHOTO>
                   <E_TELEMETRY_EVENT_ITEM_HEARTED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_ITEM_HEARTED>
                   <E_TELEMETRY_EVENT_DEATH_POSITION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_DEATH_POSITION>
                   <E_TELEMETRY_EVENT_SUICIDE_POSITION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_SUICIDE_POSITION>
                   <E_TELEMETRY_EVENT_RESTART_POSITION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_RESTART_POSITION>
                   <E_TELEMETRY_EVENT_LOST_ALL_LIVES_POSITION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_LOST_ALL_LIVES_POSITION>
                   <E_TELEMETRY_EVENT_QUIT_POSITION>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_QUIT_POSITION>
                   <E_TELEMETRY_EVENT_CONNECTION_QUALITY>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_CONNECTION_QUALITY>
                   <E_TELEMETRY_EVENT_USER_EXPERIENCE_METRICS>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_USER_EXPERIENCE_METRICS>
                   <E_TELEMETRY_EVENT_LIVE_STREAM_STARTED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_LIVE_STREAM_STARTED>
                   <E_TELEMETRY_EVENT_LIVE_STREAM_UPDATE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_LIVE_STREAM_UPDATE>
                   <E_TELEMETRY_EVENT_LIVE_STREAM_ENDED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_LIVE_STREAM_ENDED>
                   <E_TELEMETRY_EVENT_PLAY_NOW>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PLAY_NOW>
                   <E_TELEMETRY_EVENT_QUEST_ADDED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_QUEST_ADDED>
                   <E_TELEMETRY_EVENT_QUEST_COMPLETED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_QUEST_COMPLETED>
                   <E_TELEMETRY_EVENT_OBJECTIVE_COMPLETED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_OBJECTIVE_COMPLETED>
                   <E_TELEMETRY_EVENT_COMMUNITY_ACTION_UI>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_COMMUNITY_ACTION_UI>
                   <E_TELEMETRY_EVENT_TUTORIAL_VIDEO>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_TUTORIAL_VIDEO>
                   <E_TELEMETRY_EVENT_STORE_TEXT_SEARCH>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_STORE_TEXT_SEARCH>
                   <E_TELEMETRY_EVENT_COMMUNITY_TEXT_SEARCH>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_COMMUNITY_TEXT_SEARCH>
                   <E_TELEMETRY_EVENT_PIN_ACHIEVE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PIN_ACHIEVE>
                   <E_TELEMETRY_EVENT_PIN_ACHIEVE_FIRST_TIME>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_PIN_ACHIEVE_FIRST_TIME>
                   <E_TELEMETRY_EVENT_IMPORT_PHOTO>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_IMPORT_PHOTO>
                   <E_TELEMETRY_CHECKPOINT_RESPAWNED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_CHECKPOINT_RESPAWNED>
                   <E_TELEMETRY_CHECKPOINT_HIT>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_CHECKPOINT_HIT>
                   <E_TELEMETRY_CREATURE_TRACK>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_CREATURE_TRACK>
                   <E_TELEMETRY_MISSING_RESOURCE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_MISSING_RESOURCE>
                   <E_TELEMETRY_MISSING_NETWORK_RESOURCE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_MISSING_NETWORK_RESOURCE>
                   <E_TELEMETRY_TRYING_TO_PUBLISH_DLC>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_TRYING_TO_PUBLISH_DLC>
                   <E_TELEMETRY_NETWORK_JOIN_RESULT>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_NETWORK_JOIN_RESULT>
                   <E_TELEMETRY_NETWORK_ERROR>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_NETWORK_ERROR>
                   <E_TELEMETRY_ANIMATION_SELECTED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_ANIMATION_SELECTED>
                   <E_TELEMETRY_SOUND_SELECTED>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_SOUND_SELECTED>
                   <E_TELEMETRY_HACKED_LEVEL>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_HACKED_LEVEL>
                   <E_TELEMETRY_DIVE_IN_EVENT>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_DIVE_IN_EVENT>
                   <E_TELEMETRY_GENERIC_MESSAGE>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_GENERIC_MESSAGE>
                   <E_TELEMETRY_EVENT_COUNT>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_COUNT>
                   <E_TELEMETRY_EVENT_INVALID>
                       <initial_gap>0.0</initial_gap>
                       <subsequent_gap>0.0</subsequent_gap>
                   </E_TELEMETRY_EVENT_INVALID>
                   </t_timing>

                   <t_heatmaps>
                     <DISTANCE_BETWEEN_HEATMAP_UPDATES>
                       <min>0.0</min>
                       <max>100.0</max>
                     </DISTANCE_BETWEEN_HEATMAP_UPDATES>
                   </t_heatmaps>

                   """;

        return a;

        // return telemetryConfig;
    }

    private static readonly Lazy<string?> PromotionsFile
        = new(() => 
        {
            string path = Path.Combine(Environment.CurrentDirectory, "promotions.xml");

            return File.Exists(path) ? File.ReadAllText(path) : null;
        });

    [GameEndpoint("promotions")]
    [NullStatusCode(OK)]
    [MinimumRole(GameUserRole.Restricted)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string? Promotions(RequestContext context) 
    {
        bool created = PromotionsFile.IsValueCreated;
        
        string? promotions = PromotionsFile.Value;
        
        // Only log this warning once
        if(!created && promotions == null)
            context.Logger.LogWarning(BunkumCategory.Request, "promotions.xml file is missing! " +
                                                             "LBP will work without it, but it may be relevant to you if you are an advanced user.");
        
        return promotions;
    }
    
    [GameEndpoint("farc_hashes")]
    [MinimumRole(GameUserRole.Restricted)]
    //Stubbed to return a 410 Gone, so LBP3 doesn't spam us.
    //The game doesn't actually use this information for anything, so we don't allow server owners to replace this.
    public Response FarcHashes(RequestContext context) => Gone;
    
    //TODO: In the future this should allow you to have separate files per language since the game sends the language through the `language` query parameter.
    private static readonly Lazy<string?> DeveloperVideosFile
        = new(() =>
        {
            string path = Path.Combine(Environment.CurrentDirectory, "developer_videos.xml");
            
            return File.Exists(path) ? File.ReadAllText(path) : null;
        });
    
    [GameEndpoint("developer_videos")]
    [MinimumRole(GameUserRole.Restricted)]
    [NullStatusCode(OK)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string? DeveloperVideos(RequestContext context)
    {
        bool created = DeveloperVideosFile.IsValueCreated;
        
        string? developerVideos = DeveloperVideosFile.Value;
        
        // Only log this warning once
        if(!created && developerVideos == null)
            context.Logger.LogWarning(BunkumCategory.Request, "developer_videos.xml file is missing! " +
                                                              "LBP will work without it, but it may be relevant to you if you are an advanced user.");
        
        return developerVideos; 
    }
    
    [GameEndpoint("gameState", ContentType.Plaintext, HttpMethods.Post)]
    [MinimumRole(GameUserRole.Restricted)]
    // It's unknown what an "invalid" result/state would be.
    // Since it sends information like the current create mode tool in use,
    // maybe it was used as a server-side anti-cheat to detect hacks/cheats?
    // The packet captures show `VALID` being returned, so we stub this method to that.
    //
    // Example request bodies:
    // {"currentLevel": ["pod", 0],"participants":  ["turecross321","","",""]}
    // {"currentLevel": ["user_local", 59],"inCreateMode": true,"participants":  ["turecross321","","",""],"selectedCreateTool": ""}
    // {"currentLevel": ["developer_adventure_planet", 349],"inStore": true,"participants":  ["turecross321","","",""]}
    // {"highlightedSearchResult": ["level",811],"currentLevel": ["pod", 0],"inStore": true,"participants":  ["turecross321","","",""]}
    public string GameState(RequestContext context) => "VALID";

    private static readonly Lazy<string?> ChallengeConfigFile
        = new(() => 
        {
            string path = Path.Combine(Environment.CurrentDirectory, "ChallengeConfig.xml");

            return File.Exists(path) ? File.ReadAllText(path) : null;
        });
    
    [GameEndpoint("ChallengeConfig.xml", ContentType.Xml)]
    [MinimumRole(GameUserRole.Restricted)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string ChallengeConfig(RequestContext context)
    {
        bool created = ChallengeConfigFile.IsValueCreated;
        string? challengeConfig = ChallengeConfigFile.Value;
        
        // If file was read, return its contents. Else serialize and return a hard-coded default list of challenges.
        if (challengeConfig != null)
        {
            return challengeConfig;
        }
        else
        {
            // Only log this warning once
            if (!created) context.Logger.LogWarning(BunkumCategory.Request, 
                "ChallengeConfig.xml file is missing! We've defaulted to one which is loosely based off of the official server's config, "+
                "but it might be relevant to you if you are an advanced user.");
            
            using MemoryStream ms = new();
            using BunkumXmlTextWriter bunkumXmlTextWriter = new(ms);

            XmlSerializerNamespaces namespaces = new();
            namespaces.Add("", "");

            XmlSerializer serializer = new(typeof(SerializedLbp3ChallengeList));
            serializer.Serialize(bunkumXmlTextWriter, SerializedLbp3ChallengeList.Default, namespaces);
            
            ms.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new(ms);
            
            return reader.ReadToEnd();
        }
    }

    [GameEndpoint("tags")]
    [GameEndpoint("tags/popular")]
    [MinimumRole(GameUserRole.Restricted)]
    [RateLimitSettings(ConfigRequestTimeoutDuration, ConfigRequestAmount, ConfigBlockDuration, ConfigBucket)]
    public string Tags(RequestContext context) => TagExtensions.AllTags;

    // Stub this for now. Nothing will happen if this is unimplemented, and we likely won't use any data sent here for now.
    [GameEndpoint("rankData", ContentType.Plaintext, HttpMethods.Post)]
    [RequireEmailVerified]
    [DebugRequestBody]
    public Response SubmitRankData(RequestContext context, GameUser user, string body)
    {
        return NotImplemented;
    }
}