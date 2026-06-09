using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum TelemetryPlayNowSource
{
    Qrcode,
    Web,
    App,
}