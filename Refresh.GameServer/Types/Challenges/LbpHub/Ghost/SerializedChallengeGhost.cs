using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhost
{
    [XmlElement("checkpoint")] public List<SerializedChallengeCheckpointMetric> Checkpoints { get; set; } = [];
    [XmlElement("ghost_frame")] public List<SerializedChallengeGhostFrame> Frames { get; set; } = [];
    
}