using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("checkpoint")]
[XmlType("checkpoint")]
public class SerializedChallengeCheckpoint
{
    [XmlAttribute("uid")] public int Uid { get; set; }
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("metric")] public List<SerializedChallengeCheckpointMetric> Metrics { get; set; } = [];
}