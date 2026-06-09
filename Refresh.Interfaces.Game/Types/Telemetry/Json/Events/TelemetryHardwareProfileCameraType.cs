using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryHardwareProfileCameraType
{
    [EnumMember(Value = "")]
    None,
    [EnumMember(Value = "default")]
    Default,
    [EnumMember(Value = "Eyetoy1")]
    EyeToy1,
    [EnumMember(Value = "Eyetoy2")]
    EyeToy2,
    [EnumMember(Value = "USB")]
    Usb,
}
