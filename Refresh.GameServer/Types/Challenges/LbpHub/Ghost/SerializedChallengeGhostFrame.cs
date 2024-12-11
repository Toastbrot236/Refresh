using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("ghost_frame")]
[XmlType("ghost_frame")]
public class SerializedChallengeGhostFrame
{
    [XmlAttribute("time")] public int Time;
    [XmlAttribute("X")] public int X;
    [XmlAttribute("Y")] public int Y;
    [XmlAttribute("Z")] public int Z;
    [XmlAttribute("rotation")] public float Rotation;
    [XmlAttribute("keyframe")] public bool Keyframe;
}