using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryBootStartType
{
    [EnumMember(Value = "boot")]
    Boot,
    [EnumMember(Value = "liveTile")]
    LiveTile,
    [EnumMember(Value = "resume")]
    Resume,
}