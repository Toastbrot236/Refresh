using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhost
{
    [XmlElement("checkpoint")] public List<SerializedChallengeCheckpoint> Checkpoints { get; set; } = [];
    [XmlElement("ghost_frame")] public List<SerializedChallengeGhostFrame> Frames { get; set; } = [];
}