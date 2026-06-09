using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkResourceErrorType
{
    NoErrorState,
    TimeOut,
    PlayerTooBusy,
}