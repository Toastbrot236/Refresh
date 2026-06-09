using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryHardwareProfileHeadsetType
{
    [EnumMember(Value = "")]
    None,
    [EnumMember(Value = "ps4")]
    Ps4,
    [EnumMember(Value = "Bluetooth")]
    Bluetooth,
    [EnumMember(Value = "A2DP")]
    A2DP,
    [EnumMember(Value = "USB")]
    Usb,
}
