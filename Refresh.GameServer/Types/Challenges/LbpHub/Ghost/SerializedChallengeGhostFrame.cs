using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("ghost_frame")]
[XmlType("ghost_frame")]
public class SerializedChallengeGhostFrame
{
    // currently not needed
    // idea for the future: Ability to send the coordinates of these frames to a client via ApiV3 so that
    // the client could draw a nice graph of the user's movement for example
    /*
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("X")] public int LocationX { get; set; }
    [XmlAttribute("Y")] public int LocationY { get; set; }
    [XmlAttribute("Z")] public int LocationZ { get; set; }
    [XmlAttribute("rotation")] public float Rotation { get; set; }
    [XmlAttribute("keyframe")] public bool Keyframe { get; set; }
    */
}