using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryCharacter
{
    Giant,
    Sackboy,
    Oddsock,
    Bird,
    Dwarf,
}