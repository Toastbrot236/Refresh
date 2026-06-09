using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkTimeoutError
{
    [EnumMember(Value = "TIMEOUT_WAIT_FOR_JOIN_RESPONSE")]
    TimeoutWaitingForJoinResponse,
    [EnumMember(Value = "DISCONNECT_WAIT_FOR_JOIN_RESPONSE")]
    DisconnectWaitForJoinResponse,
    [EnumMember(Value = "UNKNOWN")]
    Unknown,
}