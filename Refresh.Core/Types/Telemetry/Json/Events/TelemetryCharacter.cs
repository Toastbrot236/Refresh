using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryCharacter
{
    Giant,
    Sackboy,
    Oddsock,
    Bird,
    Dwarf,
}