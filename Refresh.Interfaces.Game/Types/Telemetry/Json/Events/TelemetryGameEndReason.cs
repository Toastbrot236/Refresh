using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum TelemetryGameEndReason
{
    // fun fact for those who have somehow made their way to the loneliest place in the whole codebase,
    // this entry is duplicated in the original enum in LBP3!!
    Bug, 
    Unknown,
    Complete,
    Scored,
    Fail,
    Restart,
    Quit,
    Door,
}