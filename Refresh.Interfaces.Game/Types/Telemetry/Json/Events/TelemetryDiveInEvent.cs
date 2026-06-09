using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryDiveInEvent
{
    [EnumMember(Value = "HOST_START_SOCIAL")]
    HostStartSocial,
    [EnumMember(Value = "HOST_START_PAUSE")]
    HostStartPause,
    [EnumMember(Value = "HOST_TIMEOUT_CANCEL")]
    HostTimeoutCancel,
    [EnumMember(Value = "HOST_TIMEOUT_CONTINUE")]
    HostTimeoutContinue,
    [EnumMember(Value = "HOST_MANUAL_CANCEL")]
    HostManualCancel,
    [EnumMember(Value = "HOST_ABORTED")]
    HostAborted,
    [EnumMember(Value = "HOST_SUCESS")] // this is not a typo
    HostSuccess,
    [EnumMember(Value = "JOIN_START_SOCIAL")]
    JoinStartSocial,
    [EnumMember(Value = "JOIN_START_PAUSE")]
    JoinStartPause,
    [EnumMember(Value = "JOIN_TIMEOUT_CANCEL")]
    JoinTimeoutCancel,
    [EnumMember(Value = "JOIN_MANUAL_CANCEL")]
    JoinManualCancel,
    [EnumMember(Value = "JOIN_ABORTED")]
    JoinAborted,
    [EnumMember(Value = "JOIN_RECEIVED_HOST")]
    JoinReceivedHost,
}