using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkJoinResultResponse
{
    [EnumMember(Value = "ALLOW_JOIN")]
    AllowJoin,
    [EnumMember(Value = "DISALLOW")]
    Disallow,
    [EnumMember(Value = "TIMEOUT")]
    Timeout,
    [EnumMember(Value = "AUTO_REJECT")]
    AutoReject,
    [EnumMember(Value = "CHAT")]
    Chat,
    [EnumMember(Value = "WRONG_VERSION")]
    WrongVersion,
    [EnumMember(Value = "WRONG_BUILD_DATE")]
    WrongBuildDate,
    [EnumMember(Value = "NO_VACANCIES")]
    NoVacancies,
    [EnumMember(Value = "BUSY")]
    Busy,
    [EnumMember(Value = "TUTORIAL")]
    Tutorial,
    [EnumMember(Value = "POPPET_PAINT")]
    PoppetPaint,
    [EnumMember(Value = "BLOCKED_PLAYER_IN_PARTY")]
    BlockedPlayerInParty,
    [EnumMember(Value = "NO_NETWORK_MULTIPLAY")]
    NoNetworkMultiplay,
    [EnumMember(Value = "UNKNOWN")]
    Unknown,
}