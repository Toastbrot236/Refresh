using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPinType
{
    [EnumMember(Value = "FIRST")]
    First,
    [EnumMember(Value = "PLAY_STORY")]
    PlayStory,
    [EnumMember(Value = "PLAY_COMMUNITY")]
    PlayCommunity,
    [EnumMember(Value = "CREATE")]
    Create,
    [EnumMember(Value = "SHARE")]
    Share,
    [EnumMember(Value = "MM")]
    Mm,
    [EnumMember(Value = "MOVE")]
    Move,
    [EnumMember(Value = "MUPPET")]
    Muppet,
    [EnumMember(Value = "CROSSPLAY")]
    Crossplay,
    [EnumMember(Value = "DCCOMICS")]
    DcComics,
    [EnumMember(Value = "LBP3_STORY")]
    Lbp3Story,
    [EnumMember(Value = "LBP3_COMMUNITY")]
    Lbp3Community,
    [EnumMember(Value = "LBP3_CREATE")]
    Lbp3Create,
    [EnumMember(Value = "LBP3_SHARE")]
    Lbp3Share,
    [EnumMember(Value = "LBP3_MM")]
    Lbp3Mm,
    [EnumMember(Value = "LBP3_PS4")]
    Lbp3Ps4,
    [EnumMember(Value = "LBP3_DEPRICATED")] // typo intentional
    Lbp3Deprecated,
    [EnumMember(Value = "LBP3_JOURNEY_HOME")]
    Lbp3JourneyHome,
    [EnumMember(Value = "LBP3_CHALLENGE")]
    Lbp3Challenge,
}