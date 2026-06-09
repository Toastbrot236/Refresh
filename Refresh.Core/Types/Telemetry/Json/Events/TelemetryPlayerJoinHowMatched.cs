using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPlayerJoinHowMatched
{
    [EnumMember(Value = "CREATE")]
    Create,
    [EnumMember(Value = "HOST_MIGRATE")]
    HostMigrate,
    [EnumMember(Value = "FRIEND")]
    Friend,
    [EnumMember(Value = "INVITE")]
    Invite,
    [EnumMember(Value = "FOLLOW")]
    Follow,
    [EnumMember(Value = "HOME_GAME_LAUNCH")]
    HomeGameLaunch,
    [EnumMember(Value = "SEARCH_RESULT")]
    SearchResult,
    [EnumMember(Value = "DIVE_IN")]
    DiveIn,
    [EnumMember(Value = "UNKNOWN")]
    Unknown,
}