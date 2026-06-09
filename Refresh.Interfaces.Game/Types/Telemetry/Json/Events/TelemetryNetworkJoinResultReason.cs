using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkJoinResultReason
{
    /// <summary>
    /// Used as a generic catch-all
    /// </summary>
    [EnumMember(Value = "UNKNOWN")]
    Unknown,
    [EnumMember(Value = "NONE")]
    None,
    [EnumMember(Value = "OVERHEATING")]
    Overheating,
    [EnumMember(Value = "JOINING_ANOTHER")]
    JoiningAnother,
    [EnumMember(Value = "BLOCKING_PARTY_OPERATION")]
    BlockingPartyOperation,
    [EnumMember(Value = "SAVING")]
    Saving,
    [EnumMember(Value = "INTRO")]
    Intro,
}