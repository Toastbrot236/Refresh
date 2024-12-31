using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("ghost_frame")]
[XmlType("ghost_frame")]
public class SerializedChallengeGhostFrame
{
    // usually X - Z are integers, Rotation is a decimal number and keyframe is a boolean, but we
    // dont need to use them for anything
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("X")] public string? LocationX { get; set; }
    [XmlAttribute("Y")] public string? LocationY { get; set; }
    [XmlAttribute("Z")] public string? LocationZ { get; set; }
    [XmlAttribute("rotation")] public string? Rotation { get; set; }
    [XmlAttribute("keyframe")] public string? Keyframe { get; set; }
}