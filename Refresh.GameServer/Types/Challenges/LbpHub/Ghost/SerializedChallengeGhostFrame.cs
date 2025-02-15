using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("ghost_frame")]
[XmlType("ghost_frame")]
public class SerializedChallengeGhostFrame
{
    // currently not needed, keep class for now anyway so we could count the frames while processing
    // incoming challenge scores
    /*
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("X")] public int LocationX { get; set; }
    [XmlAttribute("Y")] public int LocationY { get; set; }
    [XmlAttribute("Z")] public int LocationZ { get; set; }
    [XmlAttribute("rotation")] public float Rotation { get; set; }
    [XmlAttribute("keyframe")] public bool Keyframe { get; set; }
    */
}