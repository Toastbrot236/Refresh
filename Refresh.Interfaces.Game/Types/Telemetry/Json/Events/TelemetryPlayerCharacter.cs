using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPlayerCharacter
{
    SackBoy,
    ToggleBig,
    ToggleSmall,
    Swoop,
    OddSock,
}