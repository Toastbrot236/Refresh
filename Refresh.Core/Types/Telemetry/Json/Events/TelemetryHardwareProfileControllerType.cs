using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum TelemetryHardwareProfileControllerType
{
    Oddpad,
    Vita,
    Pad,
    Move,
    Kbd,
    Mouse,
    Mic,
}