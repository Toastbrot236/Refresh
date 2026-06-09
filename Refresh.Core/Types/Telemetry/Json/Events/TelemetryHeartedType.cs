using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryHeartedType
{
    User,
    Level,
    Adventure,
    Playlist,
    Item,
}