using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("metric")]
[XmlType("metric")]
public class SerializedChallengeCheckpointMetric
{
    [XmlAttribute("id")] public int Id { get; set; }
    [XmlText] public long Value { get; set; }
}